using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                                 GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), item.Request.Syntax, registrationOptions.Syntax, item.Capability?.Syntax
                             )
                         );


            var methodFactory = MakeFactory(extensionMethodContext.GetRegistryParameterList(), registrationOptions.Syntax, item.Capability?.Syntax);
            var factory = methodFactory(method);

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

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, MethodDeclarationSyntax>> MakeFactory(
            ParameterListSyntax preParameterList, TypeSyntax registrationOptions, TypeSyntax? capabilityName
        )
        {
            return method => syntax => GenerateMethods(method, syntax);

            MethodDeclarationSyntax MethodFactory(MethodDeclarationSyntax method, TypeSyntax syntax, ParameterSyntax registrationParameter)
            {
                return method
                      .WithParameterList(
                           preParameterList.AddParameters(Parameter(Identifier("handler")).WithType(syntax))
                                           .AddParameters(registrationParameter)
                       );
            }

            MethodDeclarationSyntax GenerateMethods(MethodDeclarationSyntax method, TypeSyntax syntax)
            {
                if (capabilityName is { })
                {
                    return MethodFactory(
                        method, syntax,
                        Parameter(Identifier("registrationOptions"))
                           .WithType(
                                GenericName(Identifier("RegistrationOptionsDelegate")).WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList(new[] { registrationOptions, capabilityName }))
                                )
                            )
                    );
                }

                return MethodFactory(
                    method, syntax,
                    Parameter(Identifier("registrationOptions"))
                       .WithType(
                            GenericName(Identifier("RegistrationOptionsDelegate"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(SingletonSeparatedList(registrationOptions))
                                )
                        )
                );
            }
        }

        private static BlockSyntax GetNotificationRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var args = ImmutableArray.Create(requestName, registrationOptions);
            var adapterArgs = ImmutableArray.Create(requestName);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
            }

            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), "Notification", args.ToArray())
                               .WithArgumentList(
                                    GetRegistrationHandlerArgumentList(
                                        IdentifierName("registrationOptions"),
                                        registrationOptions,
                                        GetHandlerAdapterArgument(
                                            TypeArgumentList(SeparatedList(adapterArgs.ToArray())),
                                            HandlerArgument,
                                            capabilityName,
                                            false
                                        ),
                                        capabilityName,
                                        false
                                    )
                                )
                        )
                    )
                )
            );
        }
    }
}
