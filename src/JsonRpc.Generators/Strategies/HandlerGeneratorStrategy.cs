using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class HandlerGeneratorStrategy : ICompilationUnitGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(GeneratorData item)
        {
            if (item.JsonRpcAttributes.GenerateHandler is not { }) yield break;
            var members = new List<MemberDeclarationSyntax>();

            var attributesToCopy = item.TypeDeclaration.AttributeLists
                                       .Select(z => z.Attributes.Where(AttributeFilter))
                                       .Where(z => z.Any())
                                       .Select(z => AttributeList(SeparatedList(z)))
                                       .Concat(
                                            new[] {
                                                AttributeList(
                                                    SeparatedList(
                                                        new[] {
                                                            Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                                                            Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
                                                        }
                                                    )
                                                )
                                            }
                                        )
                                       .ToArray();

            var handlerInterface = InterfaceDeclaration(Identifier($"I{SpecialCasedHandlerName(item.TypeSymbol).Split('.').Last()}Handler"))
                                  .WithAttributeLists(List(attributesToCopy))
                                  .WithModifiers(item.TypeDeclaration.Modifiers)
                                  .AddBaseListTypes(
                                       SimpleBaseType(GetBaseHandlerInterface(item))
                                   )
                ;

            if (GetRegistrationAndOrCapability(item) is { } registrationAndOrCapability)
            {
                handlerInterface = handlerInterface.AddBaseListTypes(SimpleBaseType(registrationAndOrCapability));
            }

            members.Add(handlerInterface);

            var baseClass = GetBaseHandlerClass(item);
            if (baseClass is { })
            {
                var handlerClass = ClassDeclaration(Identifier($"{SpecialCasedHandlerName(item.TypeSymbol).Split('.').Last()}HandlerBase"))
                                  .WithAttributeLists(List(attributesToCopy))
                                  .WithModifiers(item.TypeDeclaration.Modifiers)
                                  .AddModifiers(Token(SyntaxKind.AbstractKeyword))
                                  .AddBaseListTypes(SimpleBaseType(baseClass));
                members.Add(handlerClass);
            }

            if (item is RequestItem request)
            {
                if (request is { PartialItem: { } })
                {
                    var handlerClass = ClassDeclaration(Identifier($"{SpecialCasedHandlerName(item.TypeSymbol).Split('.').Last()}PartialBase"))
                                      .WithAttributeLists(List(attributesToCopy))
                                      .WithModifiers(TokenList(item.TypeDeclaration.Modifiers))
                                      .AddModifiers(Token(SyntaxKind.AbstractKeyword))
                                      .AddBaseListTypes(SimpleBaseType(GetPartialResultBaseClass(request, request.PartialItem)));
                    members.Add(handlerClass);
                }
                else if (request is { PartialItems: { } })
                {
                    var handlerClass = ClassDeclaration(Identifier($"{SpecialCasedHandlerName(item.TypeSymbol).Split('.').Last()}PartialBase"))
                                      .WithAttributeLists(List(attributesToCopy))
                                      .WithModifiers(item.TypeDeclaration.Modifiers)
                                      .AddModifiers(Token(SyntaxKind.AbstractKeyword))
                                      .AddBaseListTypes(SimpleBaseType(GetPartialResultsBaseClass(request, request.PartialItems)));
                    members.Add(handlerClass);
                }
            }

            yield return NamespaceDeclaration(ParseName(item.JsonRpcAttributes.HandlerNamespace))
                        .WithMembers(List(members))
                        .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                        .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                        .NormalizeWhitespace();
        }

        private static GenericNameSyntax GetBaseHandlerInterface(GeneratorData item)
        {
            if (item is NotificationItem notification)
            {
                return GenericName("IJsonRpcNotificationHandler")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(notification.Request.Syntax)));
            }

            if (item is RequestItem request)
            {
                if (request.IsUnit)
                {
                    return GenericName("IJsonRpcRequestHandler")
                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(request.Request.Syntax)));
                }

                return GenericName("IJsonRpcRequestHandler")
                   .WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { request.Request.Syntax, request.Response.Syntax })));
            }

            throw new NotSupportedException();
        }

        private static TypeSyntax? GetBaseHandlerClass(GeneratorData item)
        {
            var onlyCapability = item is { Capability: { }, RegistrationOptions: null };
            var types = new List<TypeSyntax>() { item.Request.Syntax };
            if (item.RegistrationOptions is { })
            {
                types.Add(item.RegistrationOptions.Syntax);
            }

            if (item.Capability is { })
            {
                types.Add(item.Capability.Syntax);
            }

            if (item is NotificationItem notification)
            {
                return QualifiedName(
                    IdentifierName("AbstractHandlers"),
                    GenericName($"Notification{( onlyCapability ? "Capability" : "" )}")
                       .WithTypeArgumentList(TypeArgumentList(SeparatedList(types)))
                );
            }

            if (item is RequestItem request)
            {
                types.Insert(1, request.Response.Syntax);
                return QualifiedName(
                    IdentifierName("AbstractHandlers"),
                    GenericName($"Request{( onlyCapability ? "Capability" : "" )}")
                       .WithTypeArgumentList(TypeArgumentList(SeparatedList(types)))
                );
            }

            return null;
        }

        private static TypeSyntax GetPartialResultBaseClass(RequestItem request, SyntaxSymbol item)
        {
            var onlyCapability = request is { Capability: { }, RegistrationOptions: null };
            var types = new List<TypeSyntax> { request.Request.Syntax, request.Response.Syntax, item.Syntax };

            if (request.RegistrationOptions is { })
            {
                types.Add(request.RegistrationOptions.Syntax);
            }

            if (request.Capability is { })
            {
                types.Add(request.Capability.Syntax);
            }

            return QualifiedName(
                IdentifierName("AbstractHandlers"),
                GenericName($"PartialResult{( onlyCapability ? "Capability" : "" )}")
                   .WithTypeArgumentList(TypeArgumentList(SeparatedList(types)))
            );
        }

        private static TypeSyntax GetPartialResultsBaseClass(RequestItem request, SyntaxSymbol item)
        {
            var onlyCapability = request is { Capability: { }, RegistrationOptions: null };
            var types = new List<TypeSyntax> { request.Request.Syntax, request.Response.Syntax, item.Syntax };

            if (request.RegistrationOptions is { })
            {
                types.Add(request.RegistrationOptions.Syntax);
            }

            if (request.Capability is { })
            {
                types.Add(request.Capability.Syntax);
            }

            return QualifiedName(
                IdentifierName("AbstractHandlers"),
                GenericName($"PartialResults{( onlyCapability ? "Capability" : "" )}")
                   .WithTypeArgumentList(TypeArgumentList(SeparatedList(types)))
            );
        }

        private static GenericNameSyntax? GetRegistrationAndOrCapability(GeneratorData item)
        {
            if (item.Capability is { } && item.RegistrationOptions is { })
            {
                return GenericName("IRegistration")
                   .WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { item.RegistrationOptions.Syntax, item.Capability.Syntax })));
            }

            if (item.RegistrationOptions is { })
            {
                return GenericName("IRegistration")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(item.RegistrationOptions.Syntax)));
            }

            if (item.Capability is { })
            {
                return GenericName("ICapability")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(item.Capability.Syntax)));
            }

            return null;
        }

        private static bool AttributeFilter(AttributeSyntax syntax)
        {
            var fullString = syntax.ToFullString();
            return !fullString.Contains("Generate") && !fullString.Contains("RegistrationOptions") && !fullString.Contains("Capability");
        }
    }
}
