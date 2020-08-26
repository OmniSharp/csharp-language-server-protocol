using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit;

namespace JsonRpc.Tests
{
    public class HandlerResolverTests
    {
        public class Request : IRequest, IRequest<Response>
        {
        }

        public class Response
        {
        }

        public class Notification : IRequest
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

        [Theory]
        [InlineData(typeof(IJsonRpcRequestHandler), "request")]
        [InlineData(typeof(IJsonRpcRequestResponseHandler), "requestresponse")]
        [InlineData(typeof(IJsonRpcNotificationDataHandler), "notificationdata")]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key)
        {
            var handler = new HandlerCollection(new ServiceCollection().BuildServiceProvider(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly })) {
                (IJsonRpcHandler) Substitute.For(new[] { requestHandler }, new object[0])
            };
            handler.Should().Contain(x => x.Method == key);
        }

        [Theory]
        [InlineData(typeof(IJsonRpcRequestHandler), "request", null)]
        [InlineData(typeof(IJsonRpcRequestResponseHandler), "requestresponse", typeof(Request))]
        [InlineData(typeof(IJsonRpcNotificationDataHandler), "notificationdata", null)]
        public void Should_Have_CorrectParams(Type requestHandler, string key, Type expected)
        {
            var handler = new HandlerCollection(new ServiceCollection().BuildServiceProvider(), new HandlerTypeDescriptorProvider(new [] { typeof(HandlerTypeDescriptorProvider).Assembly, typeof(HandlerResolverTests).Assembly })) {
                (IJsonRpcHandler) Substitute.For(new[] { requestHandler }, new object[0])
            };
            handler.First(x => x.Method == key).Params.Should().IsSameOrEqualTo(expected);
        }
    }
}
