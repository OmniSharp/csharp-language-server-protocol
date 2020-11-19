using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnNotificationMethodGeneratorWithRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, ExtensionMethodData item)
        {
            if (item is not NotificationItem or { RegistrationOptions: null }) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var method = SyntaxFactory.MethodDeclaration(extensionMethodContext.Item, extensionMethodContext.GetOnMethodName())
                                      .WithModifiers(
                                           SyntaxFactory.TokenList(
                                               SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                               SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                           )
                                       )
                                      .WithBody(Helpers.GetNotificationRegistrationHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), item.Request.Syntax, item.RegistrationOptions.Syntax));

            var factory = MakeFactory(method, extensionMethodContext.GetRegistryParameterList(), SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(item.RegistrationOptions.Syntax)));

            yield return factory(DelegateHelpers.CreateAction(false, item.Request.Syntax));
            yield return factory(DelegateHelpers.CreateAsyncAction(false, item.Request.Syntax));
            yield return factory(DelegateHelpers.CreateAction(true, item.Request.Syntax));
            yield return factory(DelegateHelpers.CreateAsyncAction(true, item.Request.Syntax));

            if (item.Capability is not null)
            {
                factory = MakeFactory(
                    method
                       .WithBody(
                            Helpers.GetNotificationRegistrationHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), item.Request.Syntax, item.RegistrationOptions.Syntax, item.Capability.Syntax)
                        ),
                    extensionMethodContext.GetRegistryParameterList(),
                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { item.RegistrationOptions.Syntax, item.Capability.Syntax }))
                );
                yield return factory(DelegateHelpers.CreateAction(true, item.Request.Syntax));
                yield return factory(DelegateHelpers.CreateAsyncAction(true, item.Request.Syntax));
            }
        }

        private static Func<TypeSyntax, MethodDeclarationSyntax> MakeFactory(MethodDeclarationSyntax method, ParameterListSyntax preParameterList, TypeArgumentListSyntax typeArguments)
        {
            return DelegateHelpers.MakeMethodFactory(
                method, preParameterList, SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                        SyntaxFactory.Parameter(
                            SyntaxFactory.Identifier("registrationOptionsFactory")
                        ).WithType(
                            SyntaxFactory.GenericName(
                                              SyntaxFactory.Identifier("Func")
                                          )
                                         .WithTypeArgumentList(typeArguments)
                        )
                    )
                )
            );
        }
    }
}
