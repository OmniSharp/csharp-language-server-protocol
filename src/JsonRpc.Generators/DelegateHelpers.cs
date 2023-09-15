using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal static class DelegateHelpers
    {
        public static Func<TypeSyntax, MethodDeclarationSyntax> MakeMethodFactory(
            MethodDeclarationSyntax method, ParameterListSyntax preParameterList, ParameterListSyntax? postParameterList = null
        ) =>
            (syntax) => method
                       .WithParameterList(
                            preParameterList.AddParameters(Parameter(Identifier("handler")).WithType(syntax))
                                            .AddParameters(postParameterList?.Parameters.ToArray() ?? Array.Empty<ParameterSyntax>())
                        );

        public static Func<TypeSyntax, MethodDeclarationSyntax> MakeGenericFactory(Func<TypeSyntax, MethodDeclarationSyntax> factory, TypeSyntax constraint)
        {
            return syntax => factory(syntax)
                            .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("T")))))
                            .WithConstraintClauses(
                                 SingletonList(
                                     TypeParameterConstraintClause(IdentifierName("T"))
                                        .WithConstraints(SingletonSeparatedList<TypeParameterConstraintSyntax>(TypeConstraint(constraint)))
                                 )
                             );
        }

        public static Func<TypeSyntax, TypeSyntax?, TypeSyntax?, MethodDeclarationSyntax> MakeGenericFactory(
            Func<TypeSyntax, TypeSyntax?, TypeSyntax?, MethodDeclarationSyntax> factory, TypeSyntax constraint
        )
        {
            return (syntax, resolveSyntax, initialValueHandler) => factory(syntax, resolveSyntax, initialValueHandler)
                                                                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("T")))))
                                                                  .WithConstraintClauses(
                                                                       SingletonList(
                                                                           TypeParameterConstraintClause(IdentifierName("T"))
                                                                              .WithConstraints(SingletonSeparatedList<TypeParameterConstraintSyntax>(TypeConstraint(constraint)))
                                                                       )
                                                                   );
        }

        public static Func<TypeSyntax, TypeSyntax?, TypeSyntax?, IEnumerable<MethodDeclarationSyntax>> MakeGenericFactory(
            Func<TypeSyntax, TypeSyntax?, TypeSyntax?, IEnumerable<MethodDeclarationSyntax>> factory, TypeSyntax constraint
        )
        {
            return (syntax, resolveSyntax, initialValueHandler) => factory(syntax, resolveSyntax, initialValueHandler)
               .Select(
                    method => method
                             .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("T")))))
                             .WithConstraintClauses(
                                  SingletonList(
                                      TypeParameterConstraintClause(IdentifierName("T"))
                                         .WithConstraints(SingletonSeparatedList<TypeParameterConstraintSyntax>(TypeConstraint(constraint)))
                                  )
                              )
                );
        }

        public static Func<TypeSyntax, IEnumerable<MethodDeclarationSyntax>> MakeGenericFactory(
            Func<TypeSyntax, IEnumerable<MethodDeclarationSyntax>> factory, TypeSyntax constraint
        )
        {
            return syntax => factory(syntax)
               .Select(
                    method => method
                             .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("T")))))
                             .WithConstraintClauses(
                                  SingletonList(
                                      TypeParameterConstraintClause(IdentifierName("T"))
                                         .WithConstraints(SingletonSeparatedList<TypeParameterConstraintSyntax>(TypeConstraint(constraint)))
                                  )
                              )
                );
        }

        public static GenericNameSyntax CreateAction(bool withCancellationToken, params TypeSyntax[] arguments)
        {
            var typeArguments = arguments.ToList();
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            return GenericName(Identifier("Action"))
               .WithTypeArgumentList(TypeArgumentList(SeparatedList(typeArguments)));
        }

        public static GenericNameSyntax CreateAsyncAction(bool withCancellationToken, params TypeSyntax[] arguments) => CreateAsyncFunc(null, withCancellationToken, arguments);

        public static GenericNameSyntax CreateAsyncFunc(TypeSyntax? returnType, bool withCancellationToken, params TypeSyntax[] arguments)
        {
            var typeArguments = arguments.ToList();
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            if (returnType == null || returnType.GetSyntaxName() == "Unit")
            {
                typeArguments.Add(IdentifierName("Task"));
            }
            else
            {
                typeArguments.Add(
                    GenericName(
                        Identifier("Task"), TypeArgumentList(
                            SeparatedList(
                                new[] {
                                    returnType
                                }
                            )
                        )
                    )
                );
            }

            return GenericName(Identifier("Func"))
               .WithTypeArgumentList(TypeArgumentList(SeparatedList(typeArguments)));
        }
    }
}
