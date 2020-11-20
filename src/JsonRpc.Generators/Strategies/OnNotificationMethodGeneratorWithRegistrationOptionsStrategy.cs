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

            var methods = new List<MethodDeclarationSyntax>();

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

            methods.AddRange(factory(CreateAction(false, item.Request.Syntax)));
            methods.AddRange( factory(CreateAsyncAction(false, item.Request.Syntax)));
            methods.AddRange( factory(CreateAction(true, item.Request.Syntax)));
            methods.AddRange( factory(CreateAsyncAction(true, item.Request.Syntax)));

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, notification.Request.Syntax);
                methods.AddRange( genericFactory(CreateAction(IdentifierName("T"))));
                methods.AddRange( genericFactory(CreateAsyncAction(false, IdentifierName("T"))));
                methods.AddRange( genericFactory(CreateAction(true, IdentifierName("T"))));
                methods.AddRange( genericFactory(CreateAsyncAction(true, IdentifierName("T"))));
            }

            if (item.Capability is { } capability)
            {
                methods.AddRange( factory(CreateAction(true, item.Request.Syntax, capability.Syntax)));
                methods.AddRange( factory(CreateAsyncAction(true, item.Request.Syntax, capability.Syntax)));

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, notification.Request.Syntax);
                    methods.AddRange( genericFactory(CreateAction(true, IdentifierName("T"), capability.Syntax, capability.Syntax)));
                    methods.AddRange( genericFactory(CreateAsyncAction(true, IdentifierName("T"), capability.Syntax, capability.Syntax)));
                }
            }

            foreach (var m in methods) yield return m;
        }

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, IEnumerable<MethodDeclarationSyntax>>> MakeFactory(
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
                       )
                      .NormalizeWhitespace();
            }

            IEnumerable<MethodDeclarationSyntax> GenerateMethods(MethodDeclarationSyntax method, TypeSyntax syntax)
            {
                if (capabilityName is { })
                {
                    yield return MethodFactory(
                        method, syntax,
                        Parameter(Identifier("registrationOptions"))
                           .WithType(
                                GenericName(Identifier("Func")).WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList(new[] { capabilityName, registrationOptions }))
                                )
                            )
                    );
                }

                yield return MethodFactory(
                    method, syntax,
                    Parameter(Identifier("registrationOptions"))
                       .WithType(
                            GenericName(Identifier("Func"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(SingletonSeparatedList(registrationOptions))
                                )
                        )
                );
                yield return MethodFactory(
                    method, syntax,
                    Parameter(Identifier("registrationOptions")).WithType(registrationOptions)
                );
            };
        }

        private static BlockSyntax GetNotificationRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var args = ImmutableArray.Create(requestName, registrationOptions);
            var typeArgs = ImmutableArray.Create(registrationOptions);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
                typeArgs = typeArgs.Add(capabilityName);
            }
            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), "Notification", args.ToArray())
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"), TypeArgumentList(SeparatedList(typeArgs.ToArray()))))
                        )
                    )
                )
            );
        }
    }
}
