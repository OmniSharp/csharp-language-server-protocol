using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit;

namespace JsonRpc.Tests
{
    public class HandlerResolverTests
    {
        public class Request : IRequest, IRequest<Response> { }
        public class Response { }
        public class Notification : IRequest { }

        [Method("request")]
        public interface IJsonRpcRequestHandler : IJsonRpcRequestHandler<Request> { }

        [Method("requestresponse")]
        public interface IJsonRpcRequestResponseHandler : IJsonRpcRequestHandler<Request, Response> { }

        [Method("notificationdata")]
        public interface IJsonRpcNotificationDataHandler : IJsonRpcNotificationHandler<Notification> { }

        [Method("notification")]
        public interface IInlineJsonRpcNotificationHandler : IJsonRpcNotificationHandler { }

        [Theory]
        [InlineData(typeof(IJsonRpcRequestHandler), "request")]
        [InlineData(typeof(IJsonRpcRequestResponseHandler), "requestresponse")]
        [InlineData(typeof(IJsonRpcNotificationDataHandler), "notificationdata")]
        [InlineData(typeof(IInlineJsonRpcNotificationHandler), "notification")]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key)
        {
            var handler = new HandlerCollection();
            handler.Add((IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]));
            handler._handlers.Should().Contain(x => x.Method == key);
        }

        [Theory]
        [InlineData(typeof(IJsonRpcRequestHandler), "request", null)]
        [InlineData(typeof(IJsonRpcRequestResponseHandler), "requestresponse", typeof(Request))]
        [InlineData(typeof(IJsonRpcNotificationDataHandler), "notificationdata", null)]
        [InlineData(typeof(IInlineJsonRpcNotificationHandler), "notification", null)]
        public void Should_Have_CorrectParams(Type requestHandler, string key, Type expected)
        {
            var handler = new HandlerCollection();
            handler.Add((IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]));
            handler.First(x => x.Method == key).Params.Should().IsSameOrEqualTo(expected);
        }
    }
}
