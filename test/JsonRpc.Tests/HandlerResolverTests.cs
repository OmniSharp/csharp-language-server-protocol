using System;
using System.Linq;
using DryIoc;
using FluentAssertions;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit;

namespace JsonRpc.Tests
{
    public class HandlerResolverTests
    {
        public class Request : IRequest<Unit>, IRequest<Response>
        {
        }

        public class Response
        {
        }

        public class Notification : IRequest<Unit>
        {
        }

        [Method("request")]
        public interface IJsonRpcRequestHandler : IJsonRpcRequestHandler<Request>
        {
        }

        [Method("requestresponse")]
        public interface IJsonRpcRequestResponseHandler : IJsonRpcRequestHandler<Request, Response>
        {
        }

        [Method("notificationdata")]
        public interface IJsonRpcNotificationDataHandler : IJsonRpcNotificationHandler<Notification>
        {
        }

        [Theory(Skip = "Inaccurate")]
        [InlineData(typeof(IJsonRpcRequestHandler), "request")]
        [InlineData(typeof(IJsonRpcRequestResponseHandler), "requestresponse")]
        [InlineData(typeof(IJsonRpcNotificationDataHandler), "notificationdata")]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key)
        {
            var handler = new HandlerCollection(
                Substitute.For<IResolverContext>(),
                new AssemblyScanningHandlerTypeDescriptorProvider(
                    new[] { typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }
                )
            )
            {
                (IJsonRpcHandler)Substitute.For(new[] { requestHandler }, Array.Empty<object>())
            };
            handler.Should().Contain(x => x.Method == key);
        }

        [Theory(Skip = "Inaccurate")]
        [InlineData(typeof(IJsonRpcRequestHandler), "request", null)]
        [InlineData(typeof(IJsonRpcRequestResponseHandler), "requestresponse", typeof(Request))]
        [InlineData(typeof(IJsonRpcNotificationDataHandler), "notificationdata", null)]
        public void Should_Have_CorrectParams(Type requestHandler, string key, Type expected)
        {
            var handler = new HandlerCollection(
                Substitute.For<IResolverContext>(),
                new AssemblyScanningHandlerTypeDescriptorProvider(
                    new[] { typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly }
                )
            )
            {
                (IJsonRpcHandler)Substitute.For(new[] { requestHandler }, Array.Empty<object>())
            };
            handler.First(x => x.Method == key).Params.Should().Be(expected);
        }
    }
}
