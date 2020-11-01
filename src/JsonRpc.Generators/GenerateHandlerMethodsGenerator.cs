using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class GenerateHandlerMethodsGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            if (!( context.SyntaxReceiver is SyntaxReceiver syntaxReceiver ))
            {
                return;
            }

            var options = ( context.Compilation as CSharpCompilation )?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation;

            var generateHandlerMethodsAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerMethodsAttribute");
            var generateRequestMethodsAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateRequestMethodsAttribute");

//            context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"generateHandlerMethodsAttributeSymbol: {generateHandlerMethodsAttributeSymbol.ToDisplayString()}"));
//            context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"generateRequestMethodsAttributeSymbol: {generateRequestMethodsAttributeSymbol.ToDisplayString()}"));

            foreach (var candidateClass in syntaxReceiver.Candidates)
            {
//                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"candidate: {candidateClass.Identifier.ToFullString()}"));
                // can this be async???
                context.CancellationToken.ThrowIfCancellationRequested();

                var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(candidateClass);

                var methods = new List<MemberDeclarationSyntax>();
                var additionalUsings = new HashSet<string> {
                    "System",
                    "System.Collections.Generic",
                    "System.Threading",
                    "System.Threading.Tasks",
                    "MediatR",
                    "Microsoft.Extensions.DependencyInjection",
                };

//                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"candidate: {candidateClass.Identifier.ToFullString()}"));

                var attribute = symbol?.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateHandlerMethodsAttributeSymbol));
                if (attribute != null)
                {
                    GetExtensionHandlers(
                        context,
                        candidateClass,
                        symbol,
                        attribute,
                        methods,
                        additionalUsings
                    );
                }

                attribute = symbol?.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateRequestMethodsAttributeSymbol));
                if (attribute != null)
                {
                    GetExtensionRequestHandlers(
                        context,
                        candidateClass,
                        symbol,
                        attribute,
                        methods,
                        additionalUsings
                    );
                }

                if (!methods.Any()) continue;

                var className = GetExtensionClassName(symbol);

                var existingUsings = candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                   .Usings
                                                   .Select(x => x.WithoutTrivia())
                                                   .Union(
                                                        additionalUsings
                                                           .Except(
                                                                candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                                              .Usings.Select(z => z.Name.ToFullString())
                                                            )
                                                           .Distinct()
                                                           .Select(z => UsingDirective(IdentifierName(z)))
                                                    )
                                                   .OrderBy(x => x.Name.ToFullString())
                                                   .ToImmutableArray();

    var obsoleteAttribute = handlerInterface.AttributeLists
                                                    .SelectMany(z => z.Attributes)
                                                    .Where(z => z.Name.ToFullString() == nameof(ObsoleteAttribute) || z.Name.ToFullString() == "Obsolete")
                                                    .ToArray();            var attributes = List(
                    new[] {
                        AttributeList(
                            SeparatedList(
                                new[] {
                                    Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                                    Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute")),}.Union(obsoleteAttribute)
                        )
                                )
                            }
                        );

                var isInternal = handlerInterface.Modifiers.Any(z => z.IsKind(SyntaxKind.InternalKeyword));

                var cu = CompilationUnit(
                             List<ExternAliasDirectiveSyntax>(),
                             List(existingUsings),
                             List<AttributeListSyntax>(),
                             List<MemberDeclarationSyntax>(
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
                                               .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                                               .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                                                        .NormalizeWhitespace()
                                                 }
                                             )
                                         )
                                 }
                             )
                         )
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool))
                        .WithTrailingTrivia(CarriageReturnLineFeed)
                        .NormalizeWhitespace();

                context.AddSource(
                    $"JsonRpc_Handlers_{candidateClass.Identifier.ToFullString().Replace(".", "_")}.cs",
                    cu.SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        }


        private void GetExtensionHandlers(
            SourceGeneratorContext context,
            TypeDeclarationSyntax handlerClassOrInterface,
            INamedTypeSymbol symbol,
            AttributeData attributeData,
            List<MemberDeclarationSyntax> methods,
            HashSet<string> additionalUsings
        )
        {
            foreach (var registry in GetRegistries(attributeData, handlerClassOrInterface, symbol, context, additionalUsings))
            {
                if (IsNotification(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    methods.AddRange(HandleNotifications(handlerClassOrInterface, symbol, requestType, registry, additionalUsings, attributeData));
                }
                else if (IsRequest(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    var responseType = GetResponseType(symbol);
                    methods.AddRange(HandleRequest(handlerClassOrInterface, symbol, requestType, responseType, registry, additionalUsings, attributeData));
                }
            }
        }


        private void GetExtensionRequestHandlers(
            SourceGeneratorContext context,
            TypeDeclarationSyntax handlerClassOrInterface,
            INamedTypeSymbol symbol,
            AttributeData attributeData,
            List<MemberDeclarationSyntax> methods,
            HashSet<string> additionalUsings
        )
        {
            var registries = GetProxies(
                context,
                attributeData,
                handlerClassOrInterface,
                symbol,
                methods,
                additionalUsings
            );

            if (( attributeData.ConstructorArguments.Length == 0 ||
                  ( attributeData.ConstructorArguments[0].Kind != TypedConstantKind.Array && attributeData.ConstructorArguments[0].Value == null )
               || ( attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Array && attributeData.ConstructorArguments[0].Values.Length == 0 ) )
             && !symbol.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.NoResponseRouterProvided, handlerClassOrInterface.Identifier.GetLocation(), symbol.Name,
                        string.Join(", ", registries.Select(z => z.ToFullString()))
                    )
                );
            }

            foreach (var registry in registries)
            {
                if (IsNotification(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    methods.AddRange(HandleRequestNotifications(handlerClassOrInterface, symbol, requestType, registry, additionalUsings, attributeData));
                }

                if (IsRequest(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    var responseType = GetResponseType(symbol);
                    methods.AddRange(HandleRequestRequests(handlerClassOrInterface, symbol, requestType, responseType, registry, additionalUsings, attributeData));
                }
            }
        }

        private IEnumerable<MemberDeclarationSyntax> HandleNotifications(
            TypeDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            NameSyntax registryType,
            HashSet<string> additionalUsings,
            AttributeData attributeData
        )
        {
            var methodName = GetOnMethodName(interfaceType, attributeData);

            var parameters = ParameterList(
                SeparatedList(
                    new[] {
                        Parameter(Identifier("registry"))
                           .WithType(registryType)
                           .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                    }
                )
            );

            var capability = GetCapability(interfaceType);
            var registrationOptions = GetRegistrationOptions(interfaceType);
            if (capability != null) additionalUsings.Add(capability.ContainingNamespace.ToDisplayString());
            if (registrationOptions != null) additionalUsings.Add(registrationOptions.ContainingNamespace.ToDisplayString());

            if (registrationOptions == null)
            {
                var method = MethodDeclaration(registryType, methodName)
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithExpressionBody(GetNotificationHandlerExpression(GetMethodName(handlerInterface)))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                          .WithParameterList(
                               parameters.AddParameters(
                                   Parameter(Identifier("handler"))
                                      .WithType(syntax)
                               )
                           )
                          .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAction(false, requestType));
                yield return MakeAction(CreateAsyncAction(false, requestType));
                yield return MakeAction(CreateAction(true, requestType));
                yield return MakeAction(CreateAsyncAction(true, requestType));
                if (capability != null)
                {
                    method = method.WithExpressionBody(
                        GetNotificationCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, capability)
                    );
                    yield return MakeAction(CreateAction(requestType, capability));
                    yield return MakeAction(CreateAsyncAction(requestType, capability));
                }
            }
            else
            {
                var method = MethodDeclaration(registryType, methodName)
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithBody(GetNotificationRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions));

                var registrationParameter = Parameter(Identifier("registrationOptions"))
                   .WithType(NullableType(IdentifierName(registrationOptions.Name)));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                          .WithParameterList(
                               parameters.WithParameters(
                                   SeparatedList(
                                       parameters.Parameters.Concat(
                                           new[] { Parameter(Identifier("handler")).WithType(syntax), registrationParameter }
                                       )
                                   )
                               )
                           )
                          .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAction(false, requestType));
                yield return MakeAction(CreateAsyncAction(false, requestType));
                yield return MakeAction(CreateAction(true, requestType));
                yield return MakeAction(CreateAsyncAction(true, requestType));
                if (capability != null)
                {
                    method = method.WithBody(
                        GetNotificationRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions, capability)
                    );
                    yield return MakeAction(CreateAction(requestType, capability));
                    yield return MakeAction(CreateAsyncAction(requestType, capability));
                }
            }
        }

        private IEnumerable<MemberDeclarationSyntax> HandleRequest(
            TypeDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            TypeSyntax responseType,
            NameSyntax registryType,
            HashSet<string> additionalUsings,
            AttributeData attributeData
        )
        {
            var methodName = GetOnMethodName(interfaceType, attributeData);

            var capability = GetCapability(interfaceType);
            var registrationOptions = GetRegistrationOptions(interfaceType);
            var partialItems = GetPartialItems(requestType);
            var partialItem = GetPartialItem(requestType);
            if (capability != null) additionalUsings.Add(capability.ContainingNamespace.ToDisplayString());
            if (registrationOptions != null) additionalUsings.Add(registrationOptions.ContainingNamespace.ToDisplayString());
            if (partialItems != null) additionalUsings.Add(partialItems.ContainingNamespace.ToDisplayString());
            if (partialItem != null) additionalUsings.Add(partialItem.ContainingNamespace.ToDisplayString());

            var parameters = ParameterList(
                SeparatedList(
                    new[] {
                        Parameter(Identifier("registry"))
                           .WithType(registryType)
                           .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                    }
                )
            );

            var allowDerivedRequests = attributeData.NamedArguments
                                                    .Where(z => z.Key == "AllowDerivedRequests")
                                                    .Select(z => z.Value.Value)
                                                    .FirstOrDefault() is bool b && b;


            if (registrationOptions == null)
            {
                var method = MethodDeclaration(registryType, methodName)
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithExpressionBody(GetRequestHandlerExpression(GetMethodName(handlerInterface)))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                          .WithParameterList(
                               parameters.AddParameters(
                                   Parameter(Identifier("handler"))
                                      .WithType(syntax)
                               )
                           )
                          .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAsyncFunc(responseType, false, requestType));
                yield return MakeAction(CreateAsyncFunc(responseType, true, requestType));

                if (allowDerivedRequests)
                {
                    MemberDeclarationSyntax MakeDerivedAction(TypeSyntax syntax)
                    {
                        return method
                              .WithParameterList(
                                   parameters.WithParameters(
                                       SeparatedList(
                                           parameters.Parameters.Concat(
                                               new[] { Parameter(Identifier("handler")).WithType(syntax) }
                                           )
                                       )
                                   )
                               )
                              .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("T")))))
                              .WithConstraintClauses(
                                   SingletonList(
                                       TypeParameterConstraintClause(IdentifierName("T"))
                                          .WithConstraints(SingletonSeparatedList<TypeParameterConstraintSyntax>(TypeConstraint(ResolveTypeName(requestType))))
                                   )
                               )
                              .NormalizeWhitespace();
                    }

                    yield return MakeDerivedAction(CreateDerivedAsyncFunc(responseType, false));
                    yield return MakeDerivedAction(CreateDerivedAsyncFunc(responseType, true));
                }

                if (partialItems != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItems);
                    var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new[] { partialTypeSyntax })));

                    method = method.WithExpressionBody(GetPartialResultsHandlerExpression(GetMethodName(handlerInterface), requestType, partialTypeSyntax, responseType));

                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithExpressionBody(
                            GetPartialResultsCapabilityHandlerExpression(
                                GetMethodName(handlerInterface), requestType, responseType,
                                partialTypeSyntax, capability
                            )
                        );
                        yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, capability));
                    }
                }

                if (partialItem != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItem);

                    method = method.WithExpressionBody(GetPartialResultHandlerExpression(GetMethodName(handlerInterface), requestType, partialTypeSyntax, responseType));

                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithExpressionBody(GetPartialResultCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, partialTypeSyntax, responseType, capability));
                        yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, capability));
                    }
                }

                if (capability != null)
                {
                    method = method.WithExpressionBody(
                        GetRequestCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, capability)
                    );
                    if (responseType.ToFullString().EndsWith("Unit"))
                    {
                        method = method.WithExpressionBody(GetVoidRequestCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, capability));
                    }
                    yield return MakeAction(CreateAsyncFunc(responseType, requestType, capability));
                }
            }
            else
            {
                var method = MethodDeclaration(registryType, methodName)
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithBody(GetRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, registrationOptions));
                if (responseType.ToFullString().EndsWith("Unit"))
                {
                    method = method.WithBody(GetVoidRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions));
                }

                var registrationParameter = Parameter(Identifier("registrationOptions"))
                       .WithType(NullableType(IdentifierName(registrationOptions.Name)))
                    ;

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                          .WithParameterList(
                               parameters.WithParameters(
                                   SeparatedList(
                                       parameters.Parameters.Concat(
                                           new[] { Parameter(Identifier("handler")).WithType(syntax), registrationParameter }
                                       )
                                   )
                               )
                           )
                          .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAsyncFunc(responseType, false, requestType));
                yield return MakeAction(CreateAsyncFunc(responseType, true, requestType));

                if (allowDerivedRequests)
                {
                    MemberDeclarationSyntax MakeDerivedAction(TypeSyntax syntax)
                    {
                        return method
                              .WithParameterList(
                                   parameters.WithParameters(
                                       SeparatedList(
                                           parameters.Parameters.Concat(
                                               new[] { Parameter(Identifier("handler")).WithType(syntax), registrationParameter }
                                           )
                                       )
                                   )
                               )
                              .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("T")))))
                              .WithConstraintClauses(
                                   SingletonList(
                                       TypeParameterConstraintClause(IdentifierName("T"))
                                          .WithConstraints(SingletonSeparatedList<TypeParameterConstraintSyntax>(TypeConstraint(ResolveTypeName(requestType))))
                                   )
                               )
                              .NormalizeWhitespace();
                    }

                    yield return MakeDerivedAction(CreateDerivedAsyncFunc(responseType, false));
                    yield return MakeDerivedAction(CreateDerivedAsyncFunc(responseType, true));
                }

                if (partialItems != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItems);
                    var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new[] { partialTypeSyntax })));

                    method = method.WithBody(
                        GetPartialResultsRegistrationHandlerExpression(
                            GetMethodName(handlerInterface), requestType, responseType, partialTypeSyntax,
                            registrationOptions
                        )
                    );

                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithBody(
                            GetPartialResultsRegistrationHandlerExpression(
                                GetMethodName(handlerInterface), requestType, responseType, partialTypeSyntax,
                                registrationOptions, capability
                            )
                        );
                        yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, capability));
                    }
                }

                if (partialItem != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItem);

                    method = method.WithBody(GetPartialResultRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, partialTypeSyntax, responseType, registrationOptions));

                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithBody(
                            GetPartialResultRegistrationHandlerExpression(
                                GetMethodName(handlerInterface), requestType, partialTypeSyntax, responseType, registrationOptions,
                                capability
                            )
                        );
                        yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, capability));
                    }
                }

                if (capability != null)
                {
                    method = method.WithBody(
                        GetRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, registrationOptions, capability)
                    );
                    if (responseType.ToFullString().EndsWith("Unit"))
                    {
                        method = method.WithBody(GetVoidRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions, capability));
                    }

                    yield return MakeAction(CreateAsyncFunc(responseType, requestType, capability));
                }
            }
        }

        private static IEnumerable<NameSyntax> GetRegistries(
            AttributeData attributeData,
            TypeDeclarationSyntax interfaceSyntax,
            INamedTypeSymbol interfaceType,
            SourceGeneratorContext context,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ConstructorArguments[0].Kind != TypedConstantKind.Array)
            {
                if (attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                    return new[] { ResolveTypeName(namedTypeSymbol) };
            }
            else if (attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Array && attributeData.ConstructorArguments[0].Values.Length > 0)
            {
                return attributeData.ConstructorArguments[0].Values.Select(z => z.Value).OfType<INamedTypeSymbol>()
                                    .Select(ResolveTypeName);
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, interfaceSyntax.Identifier.GetLocation()));
                    return Enumerable.Empty<NameSyntax>();
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */
                var maskedDirection = 0b0011 & direction;


                if (maskedDirection == 1)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities");
                    return new[] { LanguageProtocolServerToClientRegistry };
                }

                if (maskedDirection == 2)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                    return new[] { LanguageProtocolClientToServerRegistry };
                }

                if (maskedDirection == 3)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                    return new[] { LanguageProtocolClientToServerRegistry, LanguageProtocolServerToClientRegistry };
                }
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, interfaceSyntax.Identifier.GetLocation()));
                    return Enumerable.Empty<NameSyntax>();
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */
                var maskedDirection = 0b0011 & direction;
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol");
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol.Client");
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol.Server");

                if (maskedDirection == 1)
                {
                    return new[] { DebugProtocolServerToClientRegistry };
                }

                if (maskedDirection == 2)
                {
                    return new[] { DebugProtocolClientToServerRegistry };
                }

                if (maskedDirection == 3)
                {
                    return new[] { DebugProtocolClientToServerRegistry, DebugProtocolServerToClientRegistry };
                }
            }

            throw new NotImplementedException("Add inference logic here " + interfaceSyntax.Identifier.ToFullString());
        }

        private static NameSyntax LanguageProtocolServerToClientRegistry { get; } =
            IdentifierName("ILanguageClientRegistry");

        private static NameSyntax LanguageProtocolClientToServerRegistry { get; } =
            IdentifierName("ILanguageServerRegistry");

        private static NameSyntax DebugProtocolServerToClientRegistry { get; } =
            IdentifierName("IDebugAdapterClientRegistry");

        private static NameSyntax DebugProtocolClientToServerRegistry { get; } =
            IdentifierName("IDebugAdapterServerRegistry");


        public static IEnumerable<NameSyntax> GetProxies(
            SourceGeneratorContext context,
            AttributeData attributeData,
            TypeDeclarationSyntax interfaceSyntax,
            INamedTypeSymbol interfaceType,
            List<MemberDeclarationSyntax> methods,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ConstructorArguments[0].Kind != TypedConstantKind.Array)
            {
                if (attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                    return new[] { ResolveTypeName(namedTypeSymbol) };
            }
            else if (attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Array && attributeData.ConstructorArguments[0].Values.Length > 0)
            {
                return attributeData.ConstructorArguments[0].Values.Select(z => z.Value).OfType<INamedTypeSymbol>()
                                    .Select(ResolveTypeName);
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, interfaceSyntax.Identifier.GetLocation()));
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
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, interfaceSyntax.Identifier.GetLocation()));
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

        private IEnumerable<MemberDeclarationSyntax> HandleRequestNotifications(
            TypeDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            NameSyntax registryType,
            HashSet<string> additionalUsings,
            AttributeData attributeData
        )
        {
            var methodName = GetSendMethodName(interfaceType, attributeData);
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

        private IEnumerable<MemberDeclarationSyntax> HandleRequestRequests(
            TypeDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            INamedTypeSymbol responseType,
            NameSyntax registryType,
            HashSet<string> additionalUsings,
            AttributeData attributeData
        )
        {
            var methodName = GetSendMethodName(interfaceType, attributeData);
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


        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                    classDeclarationSyntax.AttributeLists.Count > 0
                )
                {
                    Candidates.Add(classDeclarationSyntax);
                }

                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax &&
                    interfaceDeclarationSyntax.AttributeLists.Count > 0
                )
                {
                    Candidates.Add(interfaceDeclarationSyntax);
                }
            }
        }
    }
}
