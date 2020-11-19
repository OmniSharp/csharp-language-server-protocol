using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal static class DelegateHelpers
    {


        public static Func<TypeSyntax, MethodDeclarationSyntax> MakeMethodFactory(MethodDeclarationSyntax method, ParameterListSyntax preParameterList, ParameterListSyntax? postParameterList = null) =>
            (syntax) => method
                       .WithParameterList(preParameterList.AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("handler")).WithType(syntax)).AddParameters(postParameterList?.Parameters.ToArray() ?? Array.Empty<ParameterSyntax>()))
                       .NormalizeWhitespace();
        public static GenericNameSyntax CreateAction(bool withCancellationToken, params TypeSyntax[] arguments)
        {
            var typeArguments = arguments.ToList();
            if (withCancellationToken)
            {
                typeArguments.Add(SyntaxFactory.IdentifierName("CancellationToken"));
            }

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier("Action"))
                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeArguments)));
        }

        public static GenericNameSyntax CreateAction(params TypeSyntax[] arguments) => CreateAction(true, arguments);

        public static GenericNameSyntax CreateAsyncAction(params TypeSyntax[] arguments) => CreateAsyncFunc(null, true, arguments);

        public static GenericNameSyntax CreateAsyncAction(bool withCancellationToken, params TypeSyntax[] arguments) => CreateAsyncFunc(null, withCancellationToken, arguments);

        public static GenericNameSyntax CreateAsyncFunc(TypeSyntax? returnType, params TypeSyntax[] arguments) => CreateAsyncFunc(returnType, true, arguments);

        public static GenericNameSyntax CreateAsyncFunc(TypeSyntax? returnType, bool withCancellationToken, params TypeSyntax[] arguments)
        {
            var typeArguments = arguments.ToList();
            if (withCancellationToken)
            {
                typeArguments.Add(SyntaxFactory.IdentifierName("CancellationToken"));
            }

            if (returnType == null || returnType.ToFullString().EndsWith("Unit"))
            {
                typeArguments.Add(SyntaxFactory.IdentifierName("Task"));
            }
            else
            {
                typeArguments.Add(
                    SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("Task"), SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(
                                new[] {
                                    returnType
                                }
                            )
                        )
                    )
                );
            }

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier("Func"))
                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeArguments)));
        }

        public static GenericNameSyntax CreateGenericAsyncFunc(bool withCancellationToken, TypeSyntax? returnType)
        {
            var typeArguments = new List<TypeSyntax> {
                SyntaxFactory.IdentifierName("T")
            };
            if (withCancellationToken)
            {
                typeArguments.Add(SyntaxFactory.IdentifierName("CancellationToken"));
            }

            if (returnType == null || returnType.ToFullString().EndsWith("Unit"))
            {
                typeArguments.Add(SyntaxFactory.IdentifierName("Task"));
            }
            else
            {
                typeArguments.Add(
                    SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("Task"), SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(
                                new[] {
                                    returnType
                                }
                            )
                        )
                    )
                );
            }

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier("Func"))
                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeArguments)));
        }
    }
}
