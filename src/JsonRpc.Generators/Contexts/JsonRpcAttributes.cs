using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record JsonRpcAttributes(
        SyntaxAttributeData? GenerateHandlerMethods,
        ImmutableArray<TypeSyntax> HandlerRegistries,
        ImmutableArray<Diagnostic> HandlerRegistryDiagnostics,
        string HandlerMethodName,
        string PartialHandlerMethodName,
        bool AllowDerivedRequests,
        SyntaxAttributeData? GenerateRequestMethods,
        ImmutableArray<TypeSyntax> RequestProxies,
        ImmutableArray<Diagnostic> RequestProxyDiagnostics,
        string RequestMethodName,
        SyntaxAttributeData? GenerateHandler,
        string HandlerNamespace,
        string HandlerName,
        string ModelNamespace
    )
    {
        public static JsonRpcAttributes Parse(
            Compilation compilation,
            TypeDeclarationSyntax syntax,
            SemanticModel model,
            INamedTypeSymbol symbol,
            HashSet<string> additionalUsings
        )
        {
            var generateHandlerMethodsAttributeSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerMethodsAttribute");
            var generateRequestMethodsAttributeSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateRequestMethodsAttribute");
            var generateHandlerAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerAttribute");

            var handlerName = Helpers.SpecialCasedHandlerName(symbol).Split('.').Last();
            var attributes = new JsonRpcAttributes(
                null,
                ImmutableArray<TypeSyntax>.Empty,
                ImmutableArray<Diagnostic>.Empty,
                GetHandlerMethodName(symbol, handlerName),
                GetPartialHandlerMethodName(symbol, handlerName),
                false,
                null,
                ImmutableArray<TypeSyntax>.Empty,
                ImmutableArray<Diagnostic>.Empty,
                GetRequestMethodName(syntax, symbol, handlerName),
                null,
                symbol.ContainingNamespace.ToDisplayString(),
                handlerName,
                symbol.ContainingNamespace.ToDisplayString()
            );

            if (symbol.GetAttribute(generateHandlerAttributeSymbol) is { } generateHandlerData)
            {
                attributes = attributes with
                {
                    GenerateHandler = SyntaxAttributeData.Parse(generateHandlerData),
                    AllowDerivedRequests = generateHandlerData
                                          .NamedArguments
                                          .Select(z => z is { Key: "AllowDerivedRequests", Value: { Value: true } })
                                          .Count(z => z) is > 0,
                    HandlerNamespace = generateHandlerData is { ConstructorArguments: { Length: >= 1 } arguments }
                        ? arguments[0].Value as string ?? attributes.HandlerNamespace
                        : attributes.HandlerNamespace,
                    HandlerName = generateHandlerData is { NamedArguments: { Length: >= 1 } namedArguments }
                        ? namedArguments
                         .Select(z => z is { Key: "Name", Value: { Value: string str } } ? str : null)
                         .FirstOrDefault(z => z is { Length: > 0 }) ?? attributes.HandlerName
                        : attributes.HandlerName
                };

                attributes = attributes with
                {
                    HandlerMethodName = GetHandlerMethodName(symbol, attributes.HandlerName),
                    PartialHandlerMethodName = GetPartialHandlerMethodName(symbol, attributes.HandlerName),
                    RequestMethodName = GetRequestMethodName(syntax, symbol, attributes.HandlerName)
                };
            }

            if (symbol.GetAttribute(generateHandlerMethodsAttributeSymbol) is { } generateHandlerMethodsData)
            {
                var data = SyntaxAttributeData.Parse(generateHandlerMethodsData);
                var diagnostics = new List<Diagnostic>();
                var syntaxes = new List<TypeSyntax>();
                foreach (var registry in GetHandlerRegistries(
                             syntax,
                             generateHandlerMethodsData,
                             symbol,
                             additionalUsings
                         ))
                {
                    if (registry.diagnostic is { }) diagnostics.Add(registry.diagnostic);
                    if (registry.typeSyntax is { }) syntaxes.Add(registry.typeSyntax);
                }

                attributes = attributes with
                {
                    GenerateHandlerMethods = data,
                    HandlerMethodName = generateHandlerMethodsData
                                       .NamedArguments
                                       .Select(z => z is { Key: "MethodName", Value: { Value: string value } } ? value : null)
                                       .FirstOrDefault(z => z is not null) ?? attributes.HandlerMethodName,
                    HandlerRegistries = syntaxes.ToImmutableArray(),
                    HandlerRegistryDiagnostics = diagnostics.ToImmutableArray()
                };
            }

            if (symbol.GetAttribute(generateRequestMethodsAttributeSymbol) is { } generateRequestMethodsData)
            {
                var data = SyntaxAttributeData.Parse(generateRequestMethodsData);
                var diagnostics = new List<Diagnostic>();
                var syntaxes = new List<TypeSyntax>();
                foreach (var registry in GetRequestProxies(
                             syntax,
                             generateRequestMethodsData,
                             symbol,
                             additionalUsings
                         ))
                {
                    if (registry.diagnostic is { }) diagnostics.Add(registry.diagnostic);
                    if (registry.typeSyntax is { }) syntaxes.Add(registry.typeSyntax);
                }

                attributes = attributes with
                {
                    GenerateRequestMethods = data,
                    RequestMethodName = generateRequestMethodsData
                                       .NamedArguments
                                       .Select(z => z is { Key: "MethodName", Value: { Value: string value } } ? value : null)
                                       .FirstOrDefault(z => z is not null) ?? attributes.RequestMethodName,
                    RequestProxies = syntaxes.ToImmutableArray(),
                    RequestProxyDiagnostics = diagnostics.ToImmutableArray()
                };
            }

            return attributes;
        }

        private static IEnumerable<(TypeSyntax? typeSyntax, Diagnostic? diagnostic)> GetHandlerRegistries(
            TypeDeclarationSyntax typeDeclarationSyntax,
            AttributeData attributeData,
            INamedTypeSymbol interfaceType,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax) yield break;
            var foundValue = false;
            foreach (var item in attributeSyntax.ArgumentList?.Arguments.ToArray() ?? Array.Empty<AttributeArgumentSyntax>())
            {
                if (item.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    yield return ( typeOfExpressionSyntax.Type, null );
                    foundValue = true;
                }
            }

            if (foundValue) yield break;

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    yield return ( null, Diagnostic.Create(
                                       GeneratorDiagnostics.MissingDirection,
                                       typeDeclarationSyntax.AttributeLists.GetAttribute("GenerateHandlerMethods")?.GetLocation()
                                   )
                        );
                    yield break;
                }

                var direction = (int)interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */

                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                if (( direction & 0b0001 ) == 0b0001)
                {
                    yield return ( LanguageProtocolServerToClientRegistry, null );
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return ( LanguageProtocolClientToServerRegistry, null );
                }

                yield break;
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    yield return (
                        null,
                        Diagnostic.Create(
                            GeneratorDiagnostics.MissingDirection, typeDeclarationSyntax.AttributeLists.GetAttribute("GenerateHandlerMethods")?.GetLocation()
                        )
                    );
                    yield break;
                }

                var direction = (int)interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */

                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol");
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol.Client");
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol.Server");
                if (( direction & 0b0001 ) == 0b0001)
                {
                    yield return ( DebugProtocolServerToClientRegistry, null );
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return ( DebugProtocolClientToServerRegistry, null );
                }

                yield break;
            }

            yield return (
                null,
                Diagnostic.Create(
                    GeneratorDiagnostics.CouldNotInferRequestRouter, typeDeclarationSyntax.AttributeLists.GetAttribute("GenerateHandlerMethods")?.GetLocation()
                )
            );
        }

        private static NameSyntax LanguageProtocolServerToClientRegistry { get; } =
            SyntaxFactory.IdentifierName("ILanguageClientRegistry");

        private static NameSyntax LanguageProtocolClientToServerRegistry { get; } =
            SyntaxFactory.IdentifierName("ILanguageServerRegistry");

        private static NameSyntax DebugProtocolServerToClientRegistry { get; } =
            SyntaxFactory.IdentifierName("IDebugAdapterClientRegistry");

        private static NameSyntax DebugProtocolClientToServerRegistry { get; } =
            SyntaxFactory.IdentifierName("IDebugAdapterServerRegistry");


        private static IEnumerable<(TypeSyntax? typeSyntax, Diagnostic? diagnostic)> GetRequestProxies(
            TypeDeclarationSyntax typeDeclarationSyntax,
            AttributeData attributeData,
            INamedTypeSymbol interfaceType,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax) yield break;
            var foundValue = false;
            foreach (var item in attributeSyntax.ArgumentList?.Arguments.ToArray() ?? Array.Empty<AttributeArgumentSyntax>())
            {
                if (item.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    yield return ( typeOfExpressionSyntax.Type, null );
                    foundValue = true;
                }
            }

            if (foundValue) yield break;

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    yield return ( null,
                                   Diagnostic.Create(
                                       GeneratorDiagnostics.MissingDirection,
                                       typeDeclarationSyntax.AttributeLists.GetAttribute("GenerateRequestMethods")?.GetLocation()
                                   )
                        );
                    yield break;
                }

                var direction = (int)interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute")!.ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */

                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Models");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                if (( direction & 0b0001 ) == 0b0001)
                {
                    yield return ( LanguageProtocolServerToClient, null );
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return ( LanguageProtocolClientToServer, null );
                }

                yield break;
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    yield return ( null,
                                   Diagnostic.Create(
                                       GeneratorDiagnostics.MissingDirection,
                                       typeDeclarationSyntax.AttributeLists.GetAttribute("GenerateRequestMethods")?.GetLocation()
                                   )
                        );
                    yield break;
                }

                var direction = (int)interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol");
                additionalUsings.Add("OmniSharp.Extensions.DebugAdapter.Protocol.Models");

                if (( direction & 0b0001 ) == 0b0001)
                {
                    yield return ( DebugProtocolServerToClient, null );
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return ( DebugProtocolClientToServer, null );
                }

                yield break;
            }

            yield return ( null,
                           Diagnostic.Create(
                               GeneratorDiagnostics.CouldNotInferRequestRouter,
                               typeDeclarationSyntax.AttributeLists.GetAttribute("GenerateRequestMethods")?.GetLocation()
                           )
                );
        }

        private static NameSyntax LanguageProtocolServerToClient { get; } =
            SyntaxFactory.ParseName("ILanguageServer");

        private static NameSyntax LanguageProtocolClientToServer { get; } =
            SyntaxFactory.ParseName("ILanguageClient");

        private static NameSyntax DebugProtocolServerToClient { get; } =
            SyntaxFactory.ParseName("IDebugAdapterServer");

        private static NameSyntax DebugProtocolClientToServer { get; } =
            SyntaxFactory.ParseName("IDebugAdapterClient");

        public static string GetHandlerMethodName(INamedTypeSymbol symbol, string? handlerName)
        {
            return "On" + ( handlerName is { Length: > 0 } ? handlerName : Helpers.SpecialCasedHandlerName(symbol) );
        }

        public static string GetPartialHandlerMethodName(INamedTypeSymbol symbol, string? handlerName)
        {
            return "Observe" + ( handlerName is { Length: > 0 } ? handlerName : Helpers.SpecialCasedHandlerName(symbol) );
        }

        public static string GetRequestMethodName(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, string? handlerName)
        {
            var name = handlerName is { Length: > 0 } ? handlerName : Helpers.SpecialCasedHandlerName(symbol);
            if (
                name.StartsWith("Run")
             || name.StartsWith("Execute")
             || name.StartsWith("Set")
             || name.StartsWith("Attach")
             || name.StartsWith("Launch")
             || name.StartsWith("Read")
             || name.StartsWith("Did")
             || name.StartsWith("Log")
             || name.StartsWith("Show")
             || name.StartsWith("Register")
             || name.StartsWith("Prepare")
             || name.StartsWith("Publish")
             || name.StartsWith("ApplyWorkspaceEdit")
             || name.StartsWith("Unregister"))
            {
                return name;
            }

            if (name.EndsWith("Resolve", StringComparison.Ordinal))
            {
                return "Resolve" + name.Substring(0, name.IndexOf("Resolve", StringComparison.Ordinal));
            }

            return Helpers.IsNotification(syntax) ? "Send" + name : "Request" + name;
        }
    }
}
