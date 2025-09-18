using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    internal record RegistrationOptionAttributes(
        SyntaxAttributeData? GenerateRegistrationOptions,
        string? Key,
        ExpressionSyntax[]? KeyExpression,
        bool SupportsWorkDoneProgress,
        bool SupportsTextDocumentSelector,
        bool SupportsNotebookDocumentSelector,
        bool SupportsStaticRegistrationOptions,
        SyntaxSymbol? RegistrationOptionsConverter,
        bool ImplementsWorkDoneProgress,
        bool ImplementsTextDocumentSelector,
        bool ImplementsNotebookDocumentSelector,
        bool ImplementsStaticRegistrationOptions
    )
    {
        public static RegistrationOptionAttributes? Parse(Compilation compilation, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var registrationOptionsAttributeSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Generation.GenerateRegistrationOptionsAttribute");
            var registrationOptionsConverterAttributeSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.RegistrationOptionsConverterAttribute");
//            var registrationOptionsInterfaceSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions");
            var textDocumentRegistrationOptionsInterfaceSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions");
            var notebookDocumentRegistrationOptionsInterfaceSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.INotebookDocumentRegistrationOptions");
            var workDoneProgressOptionsInterfaceSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions");
            var staticRegistrationOptionsInterfaceSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IStaticRegistrationOptions");

            if (!( symbol.GetAttribute(registrationOptionsAttributeSymbol) is { } data )) return null;
            if (data.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax) return null;

            TypeSyntax? converterSyntax = null;
            ITypeSymbol? converter = null;

            var supportsTextDocumentSelector = data.NamedArguments.Any(z => z is { Key: nameof(SupportsTextDocumentSelector), Value: { Value: true } })
                                        || ( symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                                               z => SymbolEqualityComparer.Default.Equals(z, textDocumentRegistrationOptionsInterfaceSymbol)
                                           ) )
                                        || ( textDocumentRegistrationOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                                               type => type.Type.GetSyntaxName()?.Contains(textDocumentRegistrationOptionsInterfaceSymbol.Name) == true
                                           ) == true );
            var supportsNotebookDocumentSelector = data.NamedArguments.Any(z => z is { Key: nameof(SupportsNotebookDocumentSelector), Value: { Value: true } })
                                        || ( symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                                               z => SymbolEqualityComparer.Default.Equals(z, notebookDocumentRegistrationOptionsInterfaceSymbol)
                                           ) )
                                        || ( notebookDocumentRegistrationOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                                               type => type.Type.GetSyntaxName()?.Contains(notebookDocumentRegistrationOptionsInterfaceSymbol.Name) == true
                                           ) == true );
            var supportsWorkDoneProgress = data.NamedArguments.Any(z => z is { Key: nameof(SupportsWorkDoneProgress), Value: { Value: true } })
                                        || ( symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                                               z => SymbolEqualityComparer.Default.Equals(z, workDoneProgressOptionsInterfaceSymbol)
                                           ) )
                                        || ( workDoneProgressOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                                               type => type.Type.GetSyntaxName()?.Contains(workDoneProgressOptionsInterfaceSymbol.Name) == true
                                           ) == true );
            var supportsStaticRegistrationOptions =
                data.NamedArguments.Any(z => z is { Key: nameof(SupportsStaticRegistrationOptions), Value: { Value: true } })
             || ( symbol.AllInterfaces.Length > 0 && symbol.AllInterfaces.Any(
                    z => SymbolEqualityComparer.Default.Equals(z, staticRegistrationOptionsInterfaceSymbol)
                ) )
             || ( staticRegistrationOptionsInterfaceSymbol is { } && syntax.BaseList?.Types.Any(
                    type => type.Type.GetSyntaxName()?.Contains(staticRegistrationOptionsInterfaceSymbol.Name) == true
                ) == true );

            if (attributeSyntax is { ArgumentList: { Arguments: { Count: >= 1 } syntaxArguments } })
            {
                converter = data.NamedArguments.FirstOrDefault(z => z is { Key: "Converter" }).Value.Type;
                converterSyntax = syntaxArguments
                                 .Select(
                                      z => z is
                                      {
                                          Expression: TypeOfExpressionSyntax expressionSyntax, NameEquals: { Name: { Identifier: { Text: "Converter" } } }
                                      }
                                          ? expressionSyntax.Type
                                          : null
                                  )
                                 .FirstOrDefault();
                if (converter is null)
                {
                    if (symbol.GetAttribute(registrationOptionsConverterAttributeSymbol) is
                            { ConstructorArguments: { Length: >= 1 } converterArguments } converterData
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
            ExpressionSyntax[]? valueExpressionSyntaxes = null;
            if (data is { ConstructorArguments: { Length: > 0 } arguments } && arguments[0].Kind is TypedConstantKind.Primitive && arguments[0].Value is string)
            {
                static IEnumerable<string> getStringValue(TypedConstant constant)
                {
                    if (constant.Kind is TypedConstantKind.Primitive && constant.Value is string s)
                    {
                        yield return s;
                    }

                    if (constant.Kind is TypedConstantKind.Array)
                    {
                        foreach (var i in constant.Values.SelectMany(getStringValue))
                        {
                            yield return i;
                        }
                    }
                }

                static IEnumerable<ExpressionSyntax> getStringExpressionSyntaxes(AttributeArgumentSyntax syntax)
                {
                    switch (syntax.Expression)
                    {
                        case LiteralExpressionSyntax literalExpressionSyntax when literalExpressionSyntax.Token.IsKind(SyntaxKind.StringLiteralToken):
                            yield return literalExpressionSyntax;
                            break;
                        case InvocationExpressionSyntax
                        {
                            Expression: IdentifierNameSyntax { Identifier: { Text: "nameof" } }
                        }:
                            yield return syntax.Expression;
                            break;
                    }
                }

                value = string.Join(".", arguments.SelectMany(getStringValue));
                valueExpressionSyntaxes = attributeSyntax.ArgumentList!.Arguments.SelectMany(getStringExpressionSyntaxes).ToArray();
            }

            return new RegistrationOptionAttributes(
                new SyntaxAttributeData(attributeSyntax, data),
                value,
                valueExpressionSyntaxes,
                supportsWorkDoneProgress,
                supportsTextDocumentSelector,
                supportsNotebookDocumentSelector,
                supportsStaticRegistrationOptions,
                converterSyntax is null ? null : new SyntaxSymbol(converterSyntax, (INamedTypeSymbol)converter!),
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
                        z => notebookDocumentRegistrationOptionsInterfaceSymbol?
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
