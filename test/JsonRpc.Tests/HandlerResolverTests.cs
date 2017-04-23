using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace JsonRpc.Tests
{
    public class HandlerResolverTests
    {
        public class Request { }
        public class Response { }
        public class Notification { }

        [Method("request")]
        public interface IRequestHandler : IRequestHandler<Request> { }

        [Method("requestresponse")]
        public interface IRequestResponseHandler : IRequestHandler<Request, Response> { }

        [Method("notificationdata")]
        public interface INotificationDataHandler : INotificationHandler<Notification> { }

        [Method("notification")]
        public interface IInlineNotificationHandler : INotificationHandler { }

        [Theory]
        [InlineData(typeof(IRequestHandler), "request")]
        [InlineData(typeof(IRequestResponseHandler), "requestresponse")]
        [InlineData(typeof(INotificationDataHandler), "notificationdata")]
        [InlineData(typeof(IInlineNotificationHandler), "notification")]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key)
        {
            var handler = new HandlerCollection();
            handler.Add((IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]));
            handler._handlers.Should().Contain(x => x.Method == key);
        }

        [Theory]
        [InlineData(typeof(IRequestHandler), "request", null)]
        [InlineData(typeof(IRequestResponseHandler), "requestresponse", typeof(Request))]
        [InlineData(typeof(INotificationDataHandler), "notificationdata", null)]
        [InlineData(typeof(IInlineNotificationHandler), "notification", null)]
        public void Should_Have_CorrectParams(Type requestHandler, string key, Type expected)
        {
            var handler = new HandlerCollection();
            handler.Add((IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]));
            handler.Get(key).Params.Should().Equals(expected);
        }
    }
}