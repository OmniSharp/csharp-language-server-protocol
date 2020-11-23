using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record RegistrationOptionAttributes(
        SyntaxAttributeData? GenerateRegistrationOptions,
        string? ServerCapabilityKey,
        bool SupportsWorkDoneProgress,
        bool SupportsDocumentSelector,
        bool SupportsStaticRegistrationOptions,
        SyntaxSymbol? RegistrationOptionsConverter,
        bool ImplementsWorkDoneProgress,
        bool ImplementsDocumentSelector,
        bool ImplementsStaticRegistrationOptions
    )
    {
        public static RegistrationOptionAttributes? Parse(GeneratorExecutionContext context, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var prefix = "OmniSharp.Extensions.LanguageServer.Protocol";
            var registrationOptionsAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.GenerateRegistrationOptionsAttribute");
            var registrationOptionsConverterAttributeSymbol = context.Compilation.GetTypeByMetadataName($"{prefix}.RegistrationOptionsConverterAttribute");
//            var registrationOptionsInterfaceSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions");
            var textDocumentRegistrationOptionsInterfaceSymbol =
                context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions");
            var workDoneProgressOptionsInterfaceSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions");
            var staticRegistrationOptionsInterfaceSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IStaticRegistrationOptions");

            if (!( symbol.GetAttribute(registrationOptionsAttributeSymbol) is { } data )) return null;
            if (!( data.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax )) return null;

            TypeSyntax? converterSyntax = null;
            ITypeSymbol? converter = null;

            var supportsDocumentSelector = data.NamedArguments.Any(z => z is { Key: nameof(SupportsDocumentSelector), Value: { Value: true } })
                                    || symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                                           z => SymbolEqualityComparer.Default.Equals(z, textDocumentRegistrationOptionsInterfaceSymbol)
                                       )
                                    || textDocumentRegistrationOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                                           type => type.Type.ToFullString().Contains(textDocumentRegistrationOptionsInterfaceSymbol.Name)
                                       ) == true;
            var supportsWorkDoneProgress = data.NamedArguments.Any(z => z is { Key: nameof(SupportsWorkDoneProgress), Value: { Value: true } })
                                        || symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                                               z => SymbolEqualityComparer.Default.Equals(z, workDoneProgressOptionsInterfaceSymbol)
                                           )
                                        || workDoneProgressOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                                               type => type.Type.ToFullString().Contains(workDoneProgressOptionsInterfaceSymbol.Name)
                                           ) == true;
            var supportsStaticRegistrationOptions = data.NamedArguments.Any(z => z is { Key: nameof(SupportsStaticRegistrationOptions), Value: { Value: true } })
                                                 || symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                                                        z => SymbolEqualityComparer.Default.Equals(z, staticRegistrationOptionsInterfaceSymbol)
                                                    )
                                                 || staticRegistrationOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                                                        type => type.Type.ToFullString().Contains(staticRegistrationOptionsInterfaceSymbol.Name)
                                                    ) == true;

            if (attributeSyntax is { ArgumentList: { Arguments: { Count: >=1 } syntaxArguments } })
            {
                converter = data.NamedArguments.FirstOrDefault(z => z is { Key: "Converter" }).Value.Type;
                converterSyntax = syntaxArguments
                                 .Select(
                                      z => z is {
                                          Expression: TypeOfExpressionSyntax expressionSyntax, NameEquals: { Name: { Identifier: { Text: "Converter" } } }
                                      }
                                          ? expressionSyntax.Type
                                          : null
                                  )
                                 .FirstOrDefault();
                if (converter is null)
                {
                    if (symbol.GetAttribute(registrationOptionsConverterAttributeSymbol) is { ConstructorArguments: { Length: >=1 } converterArguments } converterData
                     && converterArguments[0].Kind is TypedConstantKind.Type
                     && converterData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax converterAttributeSyntax
                     && converterAttributeSyntax is { ArgumentList: { Arguments: { Count: 1 } converterArgumentSyntax } }
                     && converterArgumentSyntax[0].Expression is TypeOfExpressionSyntax converterTypeOfExpressionSyntax)
                    {
                        converter = converterArguments[0].Type;
                        converterSyntax = converterTypeOfExpressionSyntax.Type;
                    }
                }
            }

            string? value = null;
            if (data is { ConstructorArguments: { Length: > 0 } arguments } && arguments[0].Kind is TypedConstantKind.Primitive && arguments[0].Value is string)
            {
                value = arguments[0].Value as string;
            }

            return new RegistrationOptionAttributes(
                new SyntaxAttributeData(attributeSyntax, data),
                value,
                supportsWorkDoneProgress,
                supportsDocumentSelector,
                supportsStaticRegistrationOptions,
                converterSyntax is null ? null : new SyntaxSymbol(converterSyntax, (INamedTypeSymbol) converter!),
                symbol
                   .GetMembers()
                   .AsEnumerable()
                   .All(
                        z => workDoneProgressOptionsInterfaceSymbol?
                            .GetMembers()
                            .AsEnumerable()
                            .Any(x => SymbolEqualityComparer.Default.Equals(z, x)) == true
                    ),
                symbol
                   .GetMembers()
                   .AsEnumerable()
                   .All(
                        z => textDocumentRegistrationOptionsInterfaceSymbol?
                            .GetMembers()
                            .AsEnumerable()
                            .Any(x => SymbolEqualityComparer.Default.Equals(z, x)) == true
                    ),
                symbol
                   .GetMembers()
                   .AsEnumerable()
                   .All(
                        z => staticRegistrationOptionsInterfaceSymbol?
                            .GetMembers()
                            .AsEnumerable()
                            .Any(x => SymbolEqualityComparer.Default.Equals(z, x)) == true
                    )
            );
        }
    }
}
