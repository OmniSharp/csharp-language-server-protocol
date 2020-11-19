using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnNotificationMethodGeneratorWithoutRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, ExtensionMethodData item)
        {
            if (item is not NotificationItem or { RegistrationOptions: {} }) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var method = SyntaxFactory.MethodDeclaration(extensionMethodContext.Item, extensionMethodContext.GetOnMethodName())
                                      .WithModifiers(
                                           SyntaxFactory.TokenList(
                                               SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                               SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                           )
                                       )
                                      .WithExpressionBody(Helpers.GetNotificationHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration)))
                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            var factory = DelegateHelpers.MakeMethodFactory(method, extensionMethodContext.GetRegistryParameterList());
            yield return factory(DelegateHelpers.CreateAction(false, item.Request.Syntax));
            yield return factory(DelegateHelpers.CreateAsyncAction(false, item.Request.Syntax));
            yield return factory(DelegateHelpers.CreateAction(true, item.Request.Syntax));
            yield return factory(DelegateHelpers.CreateAsyncAction(true, item.Request.Syntax));

            if (item.Capability is not null)
            {
                factory = DelegateHelpers.MakeMethodFactory(
                    method
                       .WithExpressionBody(
                            Helpers.GetNotificationCapabilityHandlerExpression(
                                Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration),
                                item.Request.Syntax,
                                item.Capability.Syntax
                            )
                        ),
                    extensionMethodContext.GetRegistryParameterList()
                );
                // might cause issues
                yield return factory(DelegateHelpers.CreateAction(true, item.Request.Syntax, item.Capability.Syntax));
                yield return factory(DelegateHelpers.CreateAsyncAction(true, item.Request.Syntax, item.Capability.Syntax));
            }
        }
    }
}
