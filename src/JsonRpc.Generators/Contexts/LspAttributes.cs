using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record LspAttributes(
        SyntaxAttributeData? GenerateTypedDataAttribute,
        bool GenerateTypedData,
        SyntaxAttributeData? GenerateContainerAttribute,
        bool GenerateContainer,
//        SyntaxAttributeData? WorkDoneProgressAttribute,
//        SyntaxAttributeData? TextDocumentAttribute,
        SyntaxAttributeData? CapabilityAttribute,
        SyntaxSymbol? Capability,
//        SyntaxAttributeData? CapabilityKeyAttribute,
//        string? CapabilityKey,
        SyntaxAttributeData? RegistrationOptionsKeyAttribute,
        string? RegistrationOptionsKey,
        SyntaxAttributeData? RegistrationOptionsAttribute,
        SyntaxSymbol? RegistrationOptions,
        bool SupportsWorkDoneProgress,
        bool SupportsDocumentSelector,
        SyntaxSymbol? RegistrationOptionsConverter
    )
    {
        public static LspAttributes? Parse(GeneratorExecutionContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var prefix = "OmniSharp.Extensions.LanguageServer.Protocol";
//            var workDoneProgressAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.WorkDoneProgressAttribute");
//            var textDocumentAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.TextDocumentAttribute");
//            var registrationOptionsConverterAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsConverterAttribute");

            var attributes = new LspAttributes(null, false, null, false, null, null, null, null, null, null, false, false, null);

            {
                var generateTypedDataAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateTypedDataAttribute");
                if (symbol.GetAttribute(generateTypedDataAttributeSymbol) is { } data && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    attributes = attributes with {
                        GenerateTypedData = true,
                        GenerateTypedDataAttribute = new SyntaxAttributeData(attributeSyntax, data)
                    };
                }
            }
            {
                var generateContainerAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateContainerAttribute");
                if (symbol.GetAttribute(generateContainerAttributeSymbol) is { } data && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    attributes = attributes with {
                        GenerateContainer = true,
                        GenerateContainerAttribute = new SyntaxAttributeData(attributeSyntax, data)
                    };
                }
            }
            {
                var registrationOptionsKeyAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsKeyAttribute");
                if (symbol.GetAttribute(registrationOptionsKeyAttributeSymbol) is { ConstructorArguments: { Length: >=1 } arguments } data
                    && arguments[0].Kind is TypedConstantKind.Primitive && arguments[0].Value is string value
                 && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    attributes = attributes with {
                        RegistrationOptionsKeyAttribute = new SyntaxAttributeData(attributeSyntax, data),
                        RegistrationOptionsKey = value
                    };
                }
            }
            {
                var capabilityAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.CapabilityAttribute");
                if (symbol.GetAttribute(capabilityAttributeSymbol) is { ConstructorArguments: { Length: >=1 } arguments } data
                 && arguments[0].Kind is TypedConstantKind.Type && arguments[0].Value is INamedTypeSymbol value
                 && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax
                    && attributeSyntax is { ArgumentList: { Arguments: {Count: 1} syntaxArguments } }
                    && syntaxArguments[0].Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    attributes = attributes with {
                        CapabilityAttribute = new SyntaxAttributeData(attributeSyntax, data),
                        Capability = new SyntaxSymbol(typeOfExpressionSyntax.Type, value)
                    };
                }
            }
            {
                var registrationOptionsAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsAttribute");
                var registrationOptionsConverterAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsConverterAttribute");
                if (symbol.GetAttribute(registrationOptionsAttributeSymbol) is { ConstructorArguments: { Length: >=1 } arguments } data
                 && arguments[0].Kind is TypedConstantKind.Type && arguments[0].Value is INamedTypeSymbol value
                 && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax
                 && attributeSyntax is { ArgumentList: { Arguments: {Count: 1} syntaxArguments } }
                 && syntaxArguments[0].Expression is TypeOfExpressionSyntax typeOfExpressionSyntax
                    )
                {
                    var supportsDocumentSelector = data.NamedArguments.Any(z => z.Key == nameof(SupportsDocumentSelector));
                    var supportsWorkDoneProgress = data.NamedArguments.Any(z => z.Key == nameof(SupportsWorkDoneProgress));
                    var converter = data.NamedArguments.FirstOrDefault(z => z is { Key: "Converter" }).Value.Type;
                    var converterSyntax = typeOfExpressionSyntax.Type;
                    if (converter is null)
                    {
                        if (symbol.GetAttribute(registrationOptionsConverterAttributeSymbol) is { ConstructorArguments: { Length: >=1 } converterArguments } converterData
                         && converterArguments[0].Kind is TypedConstantKind.Type
                            && converterData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax converterAttributeSyntax
                         && converterAttributeSyntax  is { ArgumentList: { Arguments: {Count: 1} } converterArgumentSyntax }
                         && syntaxArguments[0].Expression is TypeOfExpressionSyntax converterTypeOfExpressionSyntax)
                        {
                            converter = converterArguments[0].Type;
                            converterSyntax = converterTypeOfExpressionSyntax.Type;
                        }
                    }
                    attributes = attributes with {
                        RegistrationOptionsAttribute = new SyntaxAttributeData(attributeSyntax, data),
                        RegistrationOptions = new SyntaxSymbol(typeOfExpressionSyntax.Type, value),
                        SupportsDocumentSelector = supportsDocumentSelector,
                        SupportsWorkDoneProgress = supportsWorkDoneProgress,
                        RegistrationOptionsConverter = new SyntaxSymbol(converterSyntax, (INamedTypeSymbol) converter!)
                    };
                }

            }

            return attributes;
        }
    }
}
