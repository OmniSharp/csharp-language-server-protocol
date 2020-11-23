using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record JsonRpcAttributes(
        SyntaxAttributeData? GenerateHandlerMethods,
        ImmutableArray<TypeSyntax> HandlerRegistries,
        string HandlerMethodName,
        string PartialHandlerMethodName,
        bool AllowDerivedRequests,
        SyntaxAttributeData? GenerateRequestMethods,
        ImmutableArray<TypeSyntax> RequestProxies,
        string RequestMethodName,
        SyntaxAttributeData? GenerateHandler,
        string HandlerNamespace,
        string HandlerName,
        string ModelNamespace
    )
    {
        public static JsonRpcAttributes Parse(GeneratorExecutionContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, HashSet<string> additionalUsings)
        {
            var generateHandlerMethodsAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerMethodsAttribute");
            var generateRequestMethodsAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateRequestMethodsAttribute");
            var generateHandlerAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerAttribute");

            var handlerName = Helpers.SpecialCasedHandlerName(symbol).Split('.').Last();
            var attributes = new JsonRpcAttributes(
                null,
                ImmutableArray<TypeSyntax>.Empty,
                GetHandlerMethodName(symbol, handlerName),
                GetPartialHandlerMethodName(symbol, handlerName),
                false,
                null,
                ImmutableArray<TypeSyntax>.Empty,
                GetRequestMethodName(syntax, symbol, handlerName),
                null,
                symbol.ContainingNamespace.ToDisplayString(),
                handlerName,
                symbol.ContainingNamespace.ToDisplayString()
            );

            if (symbol.GetAttribute(generateHandlerAttributeSymbol) is { } generateHandlerData)
            {
                attributes = attributes with {
                    GenerateHandler = SyntaxAttributeData.Parse(generateHandlerData),
                    HandlerNamespace = generateHandlerData is { ConstructorArguments: { Length: >=1 } arguments } ? arguments[0].Value as string : attributes.HandlerNamespace,
                    HandlerName = generateHandlerData is { NamedArguments: { Length: >= 1 } namedArguments } ?
                        namedArguments
                           .Select(z => z is { Key: "Name", Value: { Value:  string str } } ? str : null)
                           .FirstOrDefault(z => z is { Length: >0 }) ?? attributes.HandlerName : attributes.HandlerName
                };

                attributes = attributes with {
                    HandlerMethodName = GetHandlerMethodName(symbol, attributes.HandlerName),
                    PartialHandlerMethodName = GetPartialHandlerMethodName(symbol, attributes.HandlerName),
                    RequestMethodName = GetRequestMethodName(syntax, symbol, attributes.HandlerName)
                };
            }

            if (symbol.GetAttribute(generateHandlerMethodsAttributeSymbol) is { } generateHandlerMethodsData)
            {
                var data = SyntaxAttributeData.Parse(generateHandlerMethodsData);
                attributes = attributes with {
                    GenerateHandlerMethods = data,
                    AllowDerivedRequests = generateHandlerMethodsData
                                          .NamedArguments
                                          .Select(z => z is { Key: "AllowDerivedRequests", Value: { Value: true } })
                                          .Count(z => z) is > 0,
                    HandlerMethodName = generateHandlerMethodsData
                                       .NamedArguments
                                       .Select(z => z is { Key: "MethodName", Value: { Value: string value } } ? value : null)
                                       .FirstOrDefault(z => z is not null) ?? attributes.HandlerMethodName,
                    HandlerRegistries = GetHandlerRegistries(
                        context,
                        generateHandlerMethodsData,
                        symbol,
                        additionalUsings
                    ).ToImmutableArray()
                    };
            }

            if (symbol.GetAttribute(generateRequestMethodsAttributeSymbol) is { } generateRequestMethodsData)
            {
                var data = SyntaxAttributeData.Parse(generateRequestMethodsData);
                attributes = attributes with {
                    GenerateRequestMethods = data,
                    RequestMethodName = generateRequestMethodsData
                                       .NamedArguments
                                       .Select(z => z is { Key: "MethodName", Value: { Value: string value } } ? value : null)
                                       .FirstOrDefault(z => z is not null)?? attributes.RequestMethodName,
                    RequestProxies = GetRequestProxies(
                        context,
                        generateRequestMethodsData,
                        symbol,
                        additionalUsings
                    ).ToImmutableArray()
                    };
            }

            return attributes;
        }

        private static IEnumerable<TypeSyntax> GetHandlerRegistries(
            GeneratorExecutionContext context,
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
                    yield return typeOfExpressionSyntax.Type;
                    foundValue = true;
                }
            }

            if (foundValue) yield break;

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, attributeSyntax.GetLocation()));
                    yield break;
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

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
                    yield return LanguageProtocolServerToClientRegistry;
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return LanguageProtocolClientToServerRegistry;
                }

                yield break;
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, attributeSyntax.GetLocation()));
                    yield break;
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute").ConstructorArguments[1].Value!;

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
                    yield return DebugProtocolServerToClientRegistry;
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return DebugProtocolClientToServerRegistry;
                }

                yield break;
            }

            context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.CouldNotInferRequestRouter, attributeSyntax.GetLocation()));
        }

        private static NameSyntax LanguageProtocolServerToClientRegistry { get; } =
            SyntaxFactory.IdentifierName("ILanguageClientRegistry");

        private static NameSyntax LanguageProtocolClientToServerRegistry { get; } =
            SyntaxFactory.IdentifierName("ILanguageServerRegistry");

        private static NameSyntax DebugProtocolServerToClientRegistry { get; } =
            SyntaxFactory.IdentifierName("IDebugAdapterClientRegistry");

        private static NameSyntax DebugProtocolClientToServerRegistry { get; } =
            SyntaxFactory.IdentifierName("IDebugAdapterServerRegistry");


        private static IEnumerable<TypeSyntax> GetRequestProxies(
            GeneratorExecutionContext context,
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
                    yield return typeOfExpressionSyntax.Type;
                    foundValue = true;
                }
            }

            if (foundValue) yield break;

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.LanguageServer.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, attributeSyntax.GetLocation()));
                    yield break;
                }

                var direction = (int) interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute")!.ConstructorArguments[1].Value!;

                /*
                Unspecified = 0b0000,
                ServerToClient = 0b0001,
                ClientToServer = 0b0010,
                Bidirectional = 0b0011
                 */

                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Server");
                additionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Client");
                if (( direction & 0b0001 ) == 0b0001)
                {
                    yield return LanguageProtocolServerToClient;
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return LanguageProtocolClientToServer;
                }

                yield break;
            }

            if (interfaceType.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                var attribute = interfaceType.GetAttributes().First(z => z.AttributeClass?.Name == "MethodAttribute");
                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MissingDirection, attributeSyntax.GetLocation()));
                    yield break;
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

                if (( direction & 0b0001 ) == 0b0001)
                {
                    yield return DebugProtocolServerToClient;
                }

                if (( direction & 0b0010 ) == 0b0010)
                {
                    yield return DebugProtocolClientToServer;
                }

                yield break;
            }

            context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.CouldNotInferRequestRouter, attributeSyntax.GetLocation()));
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
            return "On" + (handlerName is { Length: > 0 } ? handlerName : Helpers.SpecialCasedHandlerName(symbol));
        }

        public static string GetPartialHandlerMethodName(INamedTypeSymbol symbol, string? handlerName)
        {
            return "Observe" + (handlerName is { Length: > 0 } ? handlerName : Helpers.SpecialCasedHandlerName(symbol));
        }

        public static string GetRequestMethodName(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, string? handlerName)
        {
            var name = handlerName is { Length: > 0 } ? handlerName : Helpers.SpecialCasedHandlerName(symbol);
            if (
                name.StartsWith("Run")
             || name.StartsWith("Execute")
                // TODO: Change this next breaking change
                // || name.StartsWith("Set")
                // || name.StartsWith("Attach")
                // || name.StartsWith("Read")
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
