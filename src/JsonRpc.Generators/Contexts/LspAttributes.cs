using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record LspAttributes(
        AttributeData? GenerateTypedDataAttribute,
        AttributeData? GenerateContainerAttribute,
        AttributeData? WorkDoneProgressAttribute,
        AttributeData? TextDocumentAttribute,
        AttributeData? CapabilityAttribute,
        AttributeData? RegistrationOptionsKeyAttribute,
        AttributeData? RegistrationOptionsAttribute,
        AttributeData? RegistrationOptionsConverterAttribute
    )
    {
        public static LspAttributes? Parse(GeneratorExecutionContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var prefix = "OmniSharp.Extensions.LanguageServer.Protocol";
            var generateTypedDataAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateTypedDataAttribute");
            var generateContainerAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateContainerAttribute");
            var workDoneProgressAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.WorkDoneProgressAttribute");
            var textDocumentAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.TextDocumentAttribute");
            var capabilityAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.CapabilityAttribute");
            var registrationOptionsKeyAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsKeyAttribute");
            var registrationOptionsAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsAttribute");
            var registrationOptionsConverterAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsConverterAttribute");

            return new LspAttributes(
                symbol.GetAttribute(generateTypedDataAttributeSymbol),
                symbol.GetAttribute(generateContainerAttributeSymbol),
                symbol.GetAttribute(workDoneProgressAttributeSymbol),
                symbol.GetAttribute(textDocumentAttributeSymbol),
                symbol.GetAttribute(capabilityAttributeSymbol),
                symbol.GetAttribute(registrationOptionsKeyAttributeSymbol),
                symbol.GetAttribute(registrationOptionsAttributeSymbol),
                symbol.GetAttribute(registrationOptionsConverterAttributeSymbol)
            );
        }
    }
}