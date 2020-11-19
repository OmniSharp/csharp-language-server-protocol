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
        AttributeData? GenerateHandlerMethods,
        ImmutableArray<NameSyntax> HandlerRegistries,
        AttributeData? GenerateRequestMethods,
        ImmutableArray<NameSyntax> RequestProxies
    )
    {
        public static JsonRpcAttributes Parse(GeneratorExecutionContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, HashSet<string> additionalUsings)
        {
            var generateHandlerMethodsAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerMethodsAttribute");
            var generateRequestMethodsAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateRequestMethodsAttribute");

            var attributes = new JsonRpcAttributes(null, ImmutableArray<NameSyntax>.Empty, null, ImmutableArray<NameSyntax>.Empty);

            var generateHandlerMethodsData = symbol.GetAttribute(generateHandlerMethodsAttributeSymbol);
            if (generateHandlerMethodsData is not null)
            {
                attributes = attributes with {
                        GenerateHandlerMethods = generateHandlerMethodsData,
                        HandlerRegistries = GetHandlerRegistries(
                            context,
                            generateHandlerMethodsData,
                            syntax,
                            symbol,
                            additionalUsings
                        ).ToImmutableArray()
                        }
                    ;
            }

            var generateRequestMethodsData = symbol.GetAttribute(generateRequestMethodsAttributeSymbol);
            if (generateRequestMethodsData is not null)
            {
                attributes = attributes with {
                    GenerateRequestMethods = generateRequestMethodsData,
                    RequestProxies = GetRequestProxies(
                        context,
                        generateRequestMethodsData,
                        syntax,
                        symbol,
                        additionalUsings
                    ).ToImmutableArray()
                    };
            }

            return attributes;
        }

        private static IEnumerable<NameSyntax> GetHandlerRegistries(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            TypeDeclarationSyntax interfaceSyntax,
            INamedTypeSymbol interfaceType,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ConstructorArguments[0].Kind != TypedConstantKind.Array)
            {
                if (attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                    return new[] { Helpers.ResolveTypeName(namedTypeSymbol) };
            }
            else if (attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Array && attributeData.ConstructorArguments[0].Values.Length > 0)
            {
                return attributeData.ConstructorArguments[0].Values.Select(z => z.Value).OfType<INamedTypeSymbol>()
                                    .Select(Helpers.ResolveTypeName);
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
            SyntaxFactory.IdentifierName("ILanguageClientRegistry");

        private static NameSyntax LanguageProtocolClientToServerRegistry { get; } =
            SyntaxFactory.IdentifierName("ILanguageServerRegistry");

        private static NameSyntax DebugProtocolServerToClientRegistry { get; } =
            SyntaxFactory.IdentifierName("IDebugAdapterClientRegistry");

        private static NameSyntax DebugProtocolClientToServerRegistry { get; } =
            SyntaxFactory.IdentifierName("IDebugAdapterServerRegistry");


        private static IEnumerable<NameSyntax> GetRequestProxies(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            TypeDeclarationSyntax interfaceSyntax,
            INamedTypeSymbol interfaceType,
            HashSet<string> additionalUsings
        )
        {
            if (attributeData.ConstructorArguments[0].Kind != TypedConstantKind.Array)
            {
                if (attributeData.ConstructorArguments[0].Value is INamedTypeSymbol namedTypeSymbol)
                    return new[] { Helpers.ResolveTypeName(namedTypeSymbol) };
            }
            else if (attributeData.ConstructorArguments[0].Kind == TypedConstantKind.Array && attributeData.ConstructorArguments[0].Values.Length > 0)
            {
                return attributeData.ConstructorArguments[0].Values.Select(z => z.Value).OfType<INamedTypeSymbol>()
                                    .Select(Helpers.ResolveTypeName);
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
            SyntaxFactory.ParseName("ILanguageServer");

        private static NameSyntax LanguageProtocolClientToServer { get; } =
            SyntaxFactory.ParseName("ILanguageClient");

        private static NameSyntax DebugProtocolServerToClient { get; } =
            SyntaxFactory.ParseName("IDebugAdapterServer");

        private static NameSyntax DebugProtocolClientToServer { get; } =
            SyntaxFactory.ParseName("IDebugAdapterClient");
    }
}
