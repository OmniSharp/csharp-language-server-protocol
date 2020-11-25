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
        SyntaxAttributeData? CapabilityAttribute,
        SyntaxSymbol? Capability,
        SyntaxAttributeData? ResolverAttribute,
        SyntaxSymbol? Resolver,
        SyntaxAttributeData? RegistrationOptionsKeyAttribute,
        string? RegistrationOptionsKey,
        SyntaxAttributeData? RegistrationOptionsAttribute,
        SyntaxSymbol? RegistrationOptions,
        bool CanBeResolved,
        bool CanHaveData
    )
    {
        public static LspAttributes? Parse(GeneratorExecutionContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var prefix = "OmniSharp.Extensions.LanguageServer.Protocol.Generation";

            var attributes = new LspAttributes(null, false, null, false, null, null,null, null, null, null, null, null, false, false);

            attributes = attributes with
                {
                CanBeResolved = syntax.BaseList?.Types.Any(z => z.ToFullString().Contains("ICanBeResolved")) == true || symbol.AllInterfaces.Any(z => z.ToDisplayString().Contains("ICanBeResolved")) == true,
                CanHaveData = syntax.BaseList?.Types.Any(z => z.ToFullString().Contains("ICanHaveData")) == true || symbol.AllInterfaces.Any(z => z.ToDisplayString().Contains("ICanHaveData")) == true,
                };

            {
                var attributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateTypedDataAttribute");
                if (symbol.GetAttribute(attributeSymbol) is { } data && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    attributes = attributes with {
                        GenerateTypedData = true,
                        GenerateTypedDataAttribute = new SyntaxAttributeData(attributeSyntax, data)
                    };
                }
            }
            {
                var attributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateContainerAttribute");
                if (symbol.GetAttribute(attributeSymbol) is { } data && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    attributes = attributes with {
                        GenerateContainer = true,
                        GenerateContainerAttribute = new SyntaxAttributeData(attributeSyntax, data)
                    };
                }
            }
            {
                var attributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsKeyAttribute");
                if (symbol.GetAttribute(attributeSymbol) is { ConstructorArguments: { Length: >=1 } arguments } data
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
                var (syntaxAttributeData, syntaxSymbol) = ExtractAttributeTypeData(symbol, context.Compilation.GetTypeByMetadataName($"{prefix}.CapabilityAttribute"));
                    attributes = attributes with {
                        CapabilityAttribute = syntaxAttributeData,
                        Capability = syntaxSymbol
                    };
            }
            {
                var (syntaxAttributeData, syntaxSymbol) = ExtractAttributeTypeData(symbol, context.Compilation.GetTypeByMetadataName($"{prefix}.ResolverAttribute"));
                attributes = attributes with {
                    ResolverAttribute = syntaxAttributeData,
                    Resolver = syntaxSymbol
                    };
            }
            {
                var (syntaxAttributeData, syntaxSymbol) = ExtractAttributeTypeData(symbol, context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsAttribute"));
                attributes = attributes with {
                    RegistrationOptionsAttribute = syntaxAttributeData,
                    RegistrationOptions = syntaxSymbol
                    };
            }

            return attributes;

            static (SyntaxAttributeData? SyntaxAttributeData, SyntaxSymbol? SyntaxSymbol) ExtractAttributeTypeData(INamedTypeSymbol symbol, INamedTypeSymbol? attributeSymbol)
            {
                if (symbol.GetAttribute(attributeSymbol) is { ConstructorArguments: { Length: >=1 } arguments } data
                 && arguments[0].Kind is TypedConstantKind.Type && arguments[0].Value is INamedTypeSymbol value
                 && data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax
                 && attributeSyntax is { ArgumentList: { Arguments: { Count: 1 } syntaxArguments } }
                 && syntaxArguments[0].Expression is TypeOfExpressionSyntax typeOfExpressionSyntax)
                {
                    return ( new SyntaxAttributeData(attributeSyntax, data), new SyntaxSymbol(typeOfExpressionSyntax.Type, value) );
                }

                return (null, null);
            }
        }
    }
}
