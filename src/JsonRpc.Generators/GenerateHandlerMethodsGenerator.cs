using System;
using System.Buffers;
using System.Collections;
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
    public class GenerateHandlerMethodsGenerator : IRichCodeGenerator
    {
        private readonly AttributeData _attributeData;

        public GenerateHandlerMethodsGenerator(AttributeData attributeData)
        {
            _attributeData = attributeData;
        }

        public Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (!(context.ProcessingNode is InterfaceDeclarationSyntax handlerInterface))
            {
                return Task.FromResult(new RichGenerationResult());
            }

            var methods = new List<MemberDeclarationSyntax>();
            var additionalUsings = new HashSet<string>() {
                "System",
                "System.Collections.Generic",
                "System.Threading",
                "System.Threading.Tasks",
                "MediatR",
                "Microsoft.Extensions.DependencyInjection",
            };
            var symbol = context.SemanticModel.GetDeclaredSymbol(handlerInterface);
            var name = SpecialCasedHandlerName(symbol);

            var className = $"{name}Extensions";

            foreach (var registry in GetRegistries(_attributeData, handlerInterface, symbol, context, progress, additionalUsings))
            {
                if (IsNotification(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    methods.AddRange(HandleNotifications(handlerInterface, symbol, requestType, registry, additionalUsings));
                }
                else if (IsRequest(symbol))
                {
                    var requestType = GetRequestType(symbol);
                    var responseType = GetResponseType(symbol);
                    methods.AddRange(HandleRequest(handlerInterface, symbol, requestType, responseType, registry, additionalUsings));
                }
            }

            var existingUsings = context.CompilationUnitUsings
                    .Join(additionalUsings, z => z.Name.ToFullString(), z => z, (a, b) => b)
                ;

            var newUsings = additionalUsings
                    .Except(existingUsings)
                    .Select(z => UsingDirective(IdentifierName(z)))
                ;

            var attributes = List(new[] {
                AttributeList(SeparatedList(new[] {
                    Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                    Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute")),
                }))
            });

            return Task.FromResult(new RichGenerationResult() {
                Usings = List(newUsings),
                Members = List<MemberDeclarationSyntax>(new[] {
                    NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))

                        .WithMembers(List(new MemberDeclarationSyntax[] {
                            ClassDeclaration(className)
                                .WithAttributeLists(attributes)
                                .WithModifiers(TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword),
                                    Token(SyntaxKind.PartialKeyword)
                                ))
                                .WithMembers(List(methods))
                                .NormalizeWhitespace()
                        }))
                })
            });
        }

        IEnumerable<MemberDeclarationSyntax> HandleNotifications(
            InterfaceDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            NameSyntax registryType,
            HashSet<string> additionalUsings)
        {
            var methodName = GetOnMethodName(interfaceType);

            var parameters = ParameterList(SeparatedList(new[] {
                Parameter(Identifier("registry"))
                    .WithType(registryType)
                    .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
            }));

            var capability = GetCapability(interfaceType);
            var registrationOptions = GetRegistrationOptions(interfaceType);
            if (capability != null) additionalUsings.Add(capability.ContainingNamespace.ToDisplayString());
            if (registrationOptions != null) additionalUsings.Add(registrationOptions.ContainingNamespace.ToDisplayString());

            if (registrationOptions == null)
            {
                var method = MethodDeclaration(registryType, methodName)
                    .WithModifiers(TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword))
                    )
                    .WithExpressionBody(GetNotificationHandlerExpression(GetMethodName(handlerInterface)))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                        .WithParameterList(parameters.AddParameters(Parameter(Identifier("handler"))
                            .WithType(syntax)))
                        .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAction(false, requestType));
                yield return MakeAction(CreateAsyncAction(false, requestType));
                yield return MakeAction(CreateAction(true, requestType));
                yield return MakeAction(CreateAsyncAction(true, requestType));
                if (capability != null)
                {
                    yield return MakeAction(CreateAction(requestType, capability));
                    yield return MakeAction(CreateAsyncAction(requestType, capability));
                    yield return MakeAction(CreateAction(requestType, capability));
                    yield return MakeAction(CreateAsyncAction(requestType, capability));
                }
            }
            else
            {
                var method = MethodDeclaration(registryType, methodName)
                    .WithModifiers(TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword))
                    )
                    .WithBody(GetNotificationRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions));

                var registrationParameter = Parameter(Identifier("registrationOptions"))
                    .WithType(IdentifierName(registrationOptions.Name));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                        .WithParameterList(parameters.WithParameters(SeparatedList(parameters.Parameters.Concat(
                            new[] {Parameter(Identifier("handler")).WithType(syntax), registrationParameter}))))
                        .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAction(false, requestType));
                yield return MakeAction(CreateAsyncAction(false, requestType));
                yield return MakeAction(CreateAction(true, requestType));
                yield return MakeAction(CreateAsyncAction(true, requestType));
                if (capability != null)
                {
                    method = method.WithBody(
                        GetNotificationRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions, capability));
                    yield return MakeAction(CreateAction(requestType, capability));
                    yield return MakeAction(CreateAsyncAction(requestType, capability));
                }
            }
        }

        IEnumerable<MemberDeclarationSyntax> HandleRequest(
            InterfaceDeclarationSyntax handlerInterface,
            INamedTypeSymbol interfaceType,
            INamedTypeSymbol requestType,
            INamedTypeSymbol responseType,
            NameSyntax registryType,
            HashSet<string> additionalUsings)
        {
            var methodName = GetOnMethodName(interfaceType);

            var capability = GetCapability(interfaceType);
            var registrationOptions = GetRegistrationOptions(interfaceType);
            var partialItems = GetPartialItems(requestType);
            var partialItem = GetPartialItem(requestType);
            if (capability != null) additionalUsings.Add(capability.ContainingNamespace.ToDisplayString());
            if (registrationOptions != null) additionalUsings.Add(registrationOptions.ContainingNamespace.ToDisplayString());
            if (partialItems != null) additionalUsings.Add(partialItems.ContainingNamespace.ToDisplayString());
            if (partialItem != null) additionalUsings.Add(partialItem.ContainingNamespace.ToDisplayString());

            var parameters = ParameterList(SeparatedList(new[] {
                Parameter(Identifier("registry"))
                    .WithType(registryType)
                    .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
            }));


            if (registrationOptions == null)
            {
                var method = MethodDeclaration(registryType, methodName)
                    .WithModifiers(TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword))
                    )
                    .WithExpressionBody(GetRequestHandlerExpression(GetMethodName(handlerInterface)))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                        .WithParameterList(parameters.AddParameters(Parameter(Identifier("handler"))
                            .WithType(syntax)))
                        .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAsyncFunc(responseType, false, requestType));
                yield return MakeAction(CreateAsyncFunc(responseType, true, requestType));
                if (partialItems != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItems);
                    var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new[] {partialTypeSyntax})));

                    method = method.WithExpressionBody(GetPartialResultsHandlerExpression(GetMethodName(handlerInterface), requestType, partialTypeSyntax, responseType));

                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithExpressionBody(GetPartialResultsCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, responseType,
                            partialTypeSyntax, capability));
                        yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, capability));
                    }
                }

                if (partialItem != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItem);

                    method = method.WithExpressionBody(GetPartialResultHandlerExpression(GetMethodName(handlerInterface), requestType, responseType));

                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithExpressionBody(GetPartialResultCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, capability));
                        yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, capability));
                    }
                }

                if (capability != null)
                {
                    method = method.WithExpressionBody(
                        GetRequestCapabilityHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, capability));
                    yield return MakeAction(CreateAsyncFunc(responseType, requestType, capability));
                }
            }
            else
            {
                var method = MethodDeclaration(registryType, methodName)
                    .WithModifiers(TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword))
                    )
                    .WithBody(GetRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, registrationOptions));
                if (responseType.Name == "Unit")
                {
                    method = method.WithBody(GetVoidRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions));
                }

                var registrationParameter = Parameter(Identifier("registrationOptions"))
                    .WithType(IdentifierName(registrationOptions.Name));

                MemberDeclarationSyntax MakeAction(TypeSyntax syntax)
                {
                    return method
                        .WithParameterList(parameters.WithParameters(SeparatedList(parameters.Parameters.Concat(
                            new[] {Parameter(Identifier("handler")).WithType(syntax), registrationParameter}))))
                        .NormalizeWhitespace();
                }

                yield return MakeAction(CreateAsyncFunc(responseType, false, requestType));
                yield return MakeAction(CreateAsyncFunc(responseType, true, requestType));

                if (partialItems != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItems);
                    var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new[] {partialTypeSyntax})));

                    method = method.WithBody(GetPartialResultsRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, partialTypeSyntax,
                        registrationOptions));

                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithBody(GetPartialResultsRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, partialTypeSyntax,
                            registrationOptions, capability));
                        yield return MakeAction(CreatePartialAction(requestType, partialItemsSyntax, capability));
                    }
                }

                if (partialItem != null)
                {
                    var partialTypeSyntax = ResolveTypeName(partialItem);

                    method = method.WithBody(GetPartialResultRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, registrationOptions));

                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, true));
                    yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, false));
                    if (capability != null)
                    {
                        method = method.WithBody(GetPartialResultRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, registrationOptions,
                            capability));
                        yield return MakeAction(CreatePartialAction(requestType, partialTypeSyntax, capability));
                    }
                }

                if (capability != null)
                {
                    method = method.WithBody(
                        GetRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, responseType, registrationOptions, capability));
                    if (responseType.Name == "Unit")
                    {
                        method = method.WithBody(GetVoidRequestRegistrationHandlerExpression(GetMethodName(handlerInterface), requestType, registrationOptions, capability));
                    }

                    yield return MakeAction(CreateAsyncFunc(responseType, requestType, capability));
                }
            }
        }

        static IEnumerable<NameSyntax> GetRegistries(
            AttributeData attributeData,
            InterfaceDeclarationSyntax interfaceSyntax,
            INamedTypeSymbol interfaceType,
            TransformationContext context,
            IProgress<Diagnostic> progress,
            HashSet<string> additionalUsings)
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
                var maskedDirection = (0b0011 & direction);


                if (maskedDirection == 1)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities");
                    return new[] {LanguageProtocolServerToClient};
                }

                if (maskedDirection == 2)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                    return new[] {LanguageProtocolClientToServer};
                }

                if (maskedDirection == 3)
                {
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities");
                    additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                    return new[] {LanguageProtocolClientToServer, LanguageProtocolServerToClient};
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
                var maskedDirection = (0b0011 & direction);
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol");

                if (maskedDirection == 1)
                {
                    return new[] {DebugProtocolServerToClient};
                }

                if (maskedDirection == 2)
                {
                    return new[] {DebugProtocolClientToServer};
                }

                if (maskedDirection == 3)
                {
                    return new[] {DebugProtocolClientToServer, DebugProtocolServerToClient};
                }
            }

            throw new NotImplementedException("Add inference logic here " + interfaceSyntax.Identifier.ToFullString());
        }

        private static NameSyntax LanguageProtocolServerToClient { get; } =
            IdentifierName("ILanguageClientRegistry");

        private static NameSyntax LanguageProtocolClientToServer { get; } =
            IdentifierName("ILanguageServerRegistry");

        private static NameSyntax DebugProtocolServerToClient { get; } =
            IdentifierName("IDebugAdapterClientRegistry");

        private static NameSyntax DebugProtocolClientToServer { get; } =
            IdentifierName("IDebugAdapterServerRegistry");

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }

    /*
     IJsonRpcNotificationHandler<in TNotification>
     IJsonRpcRequestHandler<in TRequest, TResponse>
     IJsonRpcRequestHandler<in TRequest>
     */
}
