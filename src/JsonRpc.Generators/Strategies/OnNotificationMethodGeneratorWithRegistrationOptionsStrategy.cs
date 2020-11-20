using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;
using static OmniSharp.Extensions.JsonRpc.Generators.DelegateHelpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnNotificationMethodGeneratorWithRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not { RegistrationOptions: { } registrationOptions }) yield break;
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
                        .WithBody(
                             GetNotificationRegistrationHandlerExpression(
                                 GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), item.Request.Syntax, registrationOptions.Syntax
                             )
                         );

            var factory = MakeFactory(method, extensionMethodContext.GetRegistryParameterList(), TypeArgumentList(SingletonSeparatedList(registrationOptions.Syntax)));

            yield return factory(CreateAction(false, item.Request.Syntax));
            yield return factory(CreateAsyncAction(false, item.Request.Syntax));
            yield return factory(CreateAction(true, item.Request.Syntax));
            yield return factory(CreateAsyncAction(true, item.Request.Syntax));

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, notification.Request.Syntax);
                yield return genericFactory(CreateAction(IdentifierName("T")));
                yield return genericFactory(CreateAsyncAction(false, IdentifierName("T")));
                yield return genericFactory(CreateAction(true, IdentifierName("T")));
                yield return genericFactory(CreateAsyncAction(true, IdentifierName("T")));
            }

            if (item.Capability is { } capability)
            {
                factory = MakeFactory(
                    method
                       .WithBody(
                            GetNotificationRegistrationHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), item.Request.Syntax, registrationOptions.Syntax, capability.Syntax
                            )
                        ),
                    extensionMethodContext.GetRegistryParameterList(),
                    TypeArgumentList(SeparatedList(new[] { capability.Syntax, registrationOptions.Syntax }))
                );
                yield return factory(CreateAction(true, item.Request.Syntax, capability.Syntax));
                yield return factory(CreateAsyncAction(true, item.Request.Syntax, capability.Syntax));

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, notification.Request.Syntax);
                    yield return genericFactory(CreateAction(true, IdentifierName("T"), capability.Syntax, capability.Syntax));
                    yield return genericFactory(CreateAsyncAction(true, IdentifierName("T"), capability.Syntax, capability.Syntax));
                }
            }
        }

        private static Func<TypeSyntax, MethodDeclarationSyntax> MakeFactory(
            MethodDeclarationSyntax method, ParameterListSyntax preParameterList, TypeArgumentListSyntax typeArguments
        )
        {
            return MakeMethodFactory(
                method, preParameterList, ParameterList(
                    SingletonSeparatedList<ParameterSyntax>(
                        Parameter(
                            Identifier("registrationOptionsFactory")
                        ).WithType(
                            GenericName(
                                    Identifier("Func")
                                )
                               .WithTypeArgumentList(typeArguments)
                        )
                    )
                )
            );
        }
    }
}
