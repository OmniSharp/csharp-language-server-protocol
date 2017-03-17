using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace JsonRpc.Tests
{
    public class HandlerResolverTests
    {
        class Request { }
        class Response { }
        class Notification { }

        [Method("request")]
        interface IRequestHandler : IRequestHandler<Request> { }

        [Method("requestresponse")]
        interface IRequestResponseHandler : IRequestHandler<Request, Response> { }
        class RequestResponseHandler : IRequestResponseHandler
        {
            public Task<Response> Handle(Request request)
            {
                throw new NotImplementedException();
            }
        }

        [Method("notificationdata")]
        interface INotificationDataHandler : INotificationHandler<Notification> { }

        [Method("notification")]
        interface IInlineNotificationHandler : INotificationHandler { }

        [Theory]
        [InlineData("request")]
        [InlineData("requestresponse")]
        [InlineData("notificationdata")]
        [InlineData("notification")]
        public void Should_Contain_AllDefinedMethods(string key)
        {
            var handler = new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly);
            handler._methods.Keys.Should().Contain(key);
        }

        [Theory]
        [InlineData("request", null)]
        [InlineData("requestresponse", typeof(Request))]
        [InlineData("notificationdata", null)]
        [InlineData("notification", null)]
        public void Should_Have_CorrectParams(string key, Type expected)
        {
            var handler = new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly);
            handler._methods[key].Params.Should().Equals(expected);
        }
    }
}