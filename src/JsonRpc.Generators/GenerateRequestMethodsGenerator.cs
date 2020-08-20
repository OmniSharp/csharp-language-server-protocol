using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    public class GenerateRequestMethodsGenerator : IRichCodeGenerator
    {
        private readonly AttributeData _attributeData;

        public GenerateRequestMethodsGenerator(AttributeData attributeData) => _attributeData = attributeData;

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (!( context.ProcessingNode is InterfaceDeclarationSyntax handlerInterface ))
            {
                return Task.FromResult(new RichGenerationResult());
            }

            var methods = new List<MemberDeclarationSyntax>();
            var additionalUsings = new HashSet<string> {
                "System",
                "System.Collections.Generic",
                "System.Threading",
                "System.Threading.Tasks",
                "MediatR",
                "Microsoft.Extensions.DependencyInjection",
            };
            var symbol = context.SemanticModel.GetDeclaredSymbol(handlerInterface);

            var className = GetExtensionClassName(symbol);

            var registries = GetProxies(_attributeData, handlerInterface, symbol, context, progress, additionalUsings);

            if (_attributeData.ConstructorArguments[0].Values.Length == 0 && !symbol.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                progress.Report(
                    Diagnostic.Create(
                        GeneratorDiagnostics.NoResponseRouterProvided, handlerInterface.Identifier.GetLocation(), symbol.Name,
                        string.Join(", ", registries.Select(z => z.ToFullString()))
                    )
                );
            }

            foreach (var registry in registries)
            {
                if (IsNotification(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    methods.AddRange(HandleNotifications(handlerInterface, symbol, requestType, registry, additionalUsings));
                }

                if (IsRequest(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    var responseType = GetResponseType(symbol);
                    methods.AddRange(HandleRequests(handlerInterface, symbol, requestType, responseType, registry, additionalUsings));
                }
            }


            var attributes = List(
                new[] {
                    AttributeList(
                        SeparatedList(
                            new[] {
                                Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                                Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute")),
                            }
                        )
                    )
                }
            );
            if (symbol.GetAttributes().Any(z => z.AttributeClass.Name == "GenerateRequestMethodsAttribute"))
            {
                attributes = List<AttributeListSyntax>();
            }

            var existingUsings = context.CompilationUnitUsings
                                        .Join(additionalUsings, z => z.Name.ToFullString(), z => z, (a, b) => b)
                ;

            var newUsings = additionalUsings
                           .Except(existingUsings)
                           .Select(z => UsingDirective(IdentifierName(z)))
                ;
            var isInternal = handlerInterface.Modifiers.Any(z => z.IsKind(SyntaxKind.InternalKeyword));
            return Task.FromResult(
                new RichGenerationResult {
                    Usings = List(newUsings),
                    Members = List<MemberDeclarationSyntax>(
                        new[] {
                            NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                               .WithMembers(
                                    List(
                                        new MemberDeclarationSyntax[] {
                                            ClassDeclaration(className)
                                               .WithAttributeLists(attributes)
                                               .WithModifiers(
                                                    TokenList(
                                                        new[] { isInternal ? Token(SyntaxKind.InternalKeyword) : Token(SyntaxKind.PublicKeyword) }.Concat(
                                                            new[] {
                                                                Token(SyntaxKind.StaticKeyword),
                                                                Token(SyntaxKind.PartialKeyword)
                                                            }
                                                        )
                                                    )
                                                )
                                               .WithMembers(List(methods))
                                               .NormalizeWhitespace()
                                        }
                                    )
                                )
                        }
                    )
                }
            );
        }

        public static IEnumerable<NameSyntax> GetProxies(
            AttributeData attributeData,
            InterfaceDeclarationSyntax interfaceSyntax,
            INamedTypeSymbol interfaceType,
            TransformationContext context,
            IProgress<Diagnostic> progress,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ConstructorArguments[0].Values.Length > 0)
            {
                return attributeData.ConstructorArguments[0].Values.Select(z => z.Value).OfType<INamedTypeSymbol>()
                                    .Select(ResolveTypeName);
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    progress.Report(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, interfaceSyntax.Identifier.GetLocation()));
                    return Enumerable.Empty<NameSyntax>();
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */
                var maskedDirection = 0b0011 & direction;

                if (maskedDirection == 1)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                    return new[] { LanguageProtocolServerToClient };
                }

                if (maskedDirection == 2)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                    return new[] { LanguageProtocolClientToServer };
                }

                if (maskedDirection == 3)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                    return new[] { LanguageProtocolClientToServer, LanguageProtocolServerToClient };
                }
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    progress.Report(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, interfaceSyntax.Identifier.GetLocation()));
                    return Enumerable.Empty<NameSyntax>();
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */
                var maskedDirection = 0b0011 & direction;
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol");

                if (maskedDirection == 1)
                {
                    return new[] { DebugProtocolServerToClient };
                }

                if (maskedDirection == 2)
                {
                    return new[] { DebugProtocolClientToServer };
                }

                if (maskedDirection == 3)
                {
                    return new[] { DebugProtocolClientToServer, DebugProtocolServerToClient };
                }
            }

            throw new NotImplementedException("Add inference logic here " + interfaceSyntax.Identifier.ToFullString());
        }

        private static NameSyntax LanguageProtocolServerToClient { get; } =
            ParseName("ILanguageServer");

        private static NameSyntax LanguageProtocolClientToServer { get; } =
            ParseName("ILanguageClient");

        private static NameSyntax DebugProtocolServerToClient { get; } =
            ParseName("IDebugAdapterServer");

        private static NameSyntax DebugProtocolClientToServer { get; } =
            ParseName("IDebugAdapterClient");

        private IEnumerable<MemberDeclarationSyntax> HandleNotifications(
            InterfaceDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            NameSyntax registryType,
            HashSet<string> additionalUsings
        )
        {
            var methodName = GetSendMethodName(interfaceType, _attributeData);
            var method = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), methodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithExpressionBody(GetNotificationInvokeExpression())
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            yield return method
                        .WithParameterList(
                             ParameterList(
                                 SeparatedList(
                                     new[] {
                                         Parameter(Identifier("mediator"))
                                            .WithType(registryType)
                                            .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                                         Parameter(Identifier("@params"))
                                            .WithType(IdentifierName(requestType.Name))
                                     }
                                 )
                             )
                         )
                        .NormalizeWhitespace();
        }

        private IEnumerable<MemberDeclarationSyntax> HandleRequests(
            InterfaceDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            INamedTypeSymbol responseType,
            NameSyntax registryType,
            HashSet<string> additionalUsings
        )
        {
            var methodName = GetSendMethodName(interfaceType, _attributeData);
            var parameterList = ParameterList(
                SeparatedList(
                    new[] {
                        Parameter(Identifier("mediator"))
                           .WithType(registryType)
                           .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                        Parameter(Identifier("@params"))
                           .WithType(IdentifierName(requestType.Name)),
                        Parameter(Identifier("cancellationToken"))
                           .WithType(IdentifierName("CancellationToken"))
                           .WithDefault(
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
                                )
                            )
                    }
                )
            );
            var partialItem = GetPartialItem(requestType);
            if (partialItem != null)
            {
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
                var partialTypeSyntax = ResolveTypeName(partialItem);
                yield return MethodDeclaration(
                                 GenericName(
                                         Identifier("IRequestProgressObservable")
                                     )
                                    .WithTypeArgumentList(
                                         TypeArgumentList(
                                             SeparatedList(
                                                 new TypeSyntax[] {
                                                     partialTypeSyntax,
                                                     ResolveTypeName(responseType)
                                                 }
                                             )
                                         )
                                     ),
                                 Identifier(methodName)
                             )
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithParameterList(parameterList)
                            .WithExpressionBody(GetPartialInvokeExpression(ResolveTypeName(responseType)))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            .NormalizeWhitespace();
                yield break;
            }

            var partialItems = GetPartialItems(requestType);
            if (partialItems != null)
            {
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
                var partialTypeSyntax = ResolveTypeName(partialItems);
                var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new[] { partialTypeSyntax })));
                yield return MethodDeclaration(
                                 GenericName(
                                         Identifier("IRequestProgressObservable")
                                     )
                                    .WithTypeArgumentList(
                                         TypeArgumentList(
                                             SeparatedList(
                                                 new TypeSyntax[] {
                                                     partialItemsSyntax,
                                                     ResolveTypeName(responseType)
                                                 }
                                             )
                                         )
                                     ),
                                 Identifier(methodName)
                             )
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithParameterList(parameterList)
                            .WithExpressionBody(GetPartialInvokeExpression(ResolveTypeName(responseType)))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            .NormalizeWhitespace();
                ;
                yield break;
            }


            var responseSyntax = responseType.Name == "Unit"
                ? IdentifierName("Task") as NameSyntax
                : GenericName("Task").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new[] { ResolveTypeName(responseType) })));
            yield return MethodDeclaration(responseSyntax, methodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithParameterList(parameterList)
                        .WithExpressionBody(GetRequestInvokeExpression())
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        .NormalizeWhitespace();
        }
    }
}
