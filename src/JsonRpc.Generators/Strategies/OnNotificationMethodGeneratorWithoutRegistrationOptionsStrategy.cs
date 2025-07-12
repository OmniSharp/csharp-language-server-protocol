using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;
using static OmniSharp.Extensions.JsonRpc.Generators.DelegateHelpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnNotificationMethodGeneratorWithoutRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is { RegistrationOptions: { } }) yield break;
            if (item is not NotificationItem notification) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var allowDerivedRequests = item.JsonRpcAttributes.AllowDerivedRequests;

            var method = MethodDeclaration(extensionMethodContext.Item, item.JsonRpcAttributes.HandlerMethodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithExpressionBody(GetNotificationHandlerExpression(GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration)))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            var factory = MakeMethodFactory(method, extensionMethodContext.GetRegistryParameterList());
            yield return factory(CreateAction(false, item.Request.Syntax));
            yield return factory(CreateAsyncAction(false, item.Request.Syntax));
            yield return factory(CreateAction(true, item.Request.Syntax));
            yield return factory(CreateAsyncAction(true, item.Request.Syntax));

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, notification.Request.Syntax);
                yield return genericFactory(CreateAction(false, IdentifierName("T")));
                yield return genericFactory(CreateAction(true, IdentifierName("T")));
                yield return genericFactory(CreateAsyncAction(false, IdentifierName("T")));
                yield return genericFactory(CreateAsyncAction(true, IdentifierName("T")));
            }

            if (item.Capability is not null)
            {
                factory = MakeMethodFactory(
                    method
                       .WithExpressionBody(
                            GetNotificationCapabilityHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration),
                                item.Request.Syntax,
                                item.Capability.Syntax
                            )
                        ),
                    extensionMethodContext.GetRegistryParameterList()
                );
                // might cause issues
                yield return factory(CreateAction(true, item.Request.Syntax, item.Capability.Syntax));
                yield return factory(CreateAsyncAction(true, item.Request.Syntax, item.Capability.Syntax));

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, IdentifierName("T"));
                    yield return genericFactory(CreateAction(true, IdentifierName("T"), item.Capability.Syntax));
                    yield return genericFactory(CreateAsyncAction(true, IdentifierName("T"), item.Capability.Syntax));
                }
            }
        }
    }
}
