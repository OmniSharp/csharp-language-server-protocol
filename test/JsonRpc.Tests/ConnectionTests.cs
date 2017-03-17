using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JsonRPC;
using JsonRPC.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;

namespace Lsp.Tests
{
    public class InputHandlerTests
    {
        [Fact]
        public async Task Test1()
        {
            var inputStream = new MemoryStream(Encoding.ASCII.GetBytes("Content-Length: 2\r\n\r\n{}"));
            var inputWriter = new StreamWriter(inputStream);

            var cts = new CancellationTokenSource();
            using (var inputHandler = new InputHandler(new StreamReader(inputStream), (token) => {
                token.Should().BeEquivalentTo("{}");
                cts.Cancel();
            }))
            {
                cts.Wait();
            }
        }
    }

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
        [InlineData("request", false)]
        [InlineData("requestresponse", true)]
        [InlineData("notificationdata", false)]
        [InlineData("notification", false)]
        public void Should_Indiciate_WhatIsImplemented(string key, bool expected)
        {
            var handler = new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly);
            handler._methods[key].IsImplemented.Should().Be(expected);
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

    public static class TestExtensions
    {
        public static void Wait(this CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Token.WaitHandle.WaitOne();
        }
    }

    public class MediatorTests_RequestHandlerOfTRequest
    {
        [Method("workspace/executeCommand")]
        interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class ExecuteCommandParams
        {
            public string Command { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var id = Guid.NewGuid();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);


        }

    }

    public class MediatorTests_RequestHandlerOfTRequestTResponse
    {
        [Method("textDocument/codeAction")]
        interface ICodeActionHandler : IRequestHandler<CodeActionParams, IEnumerable<Command>> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class CodeActionParams
        {
            public string TextDocument { get; set; }
            public string Range { get; set; }
            public string Context { get; set; }
        }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class Command
        {
            public string Title { get; set; }
            [JsonProperty("command")]
            public string Name { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);
            
            var id = Guid.NewGuid();
            var @params = new CodeActionParams() { TextDocument = "TextDocument", Range = "Range", Context = "Context" };
            var request = new Request(id, "textDocument/codeAction", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.HandleRequest(request);
        }

    }
    public class MediatorTests_NotificationHandlerOfT
    {
        [Method("$/cancelRequest")]
        interface ICancelRequestHandler : INotificationHandler<CancelParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        class CancelParams
        {
            public object Id { get; set; }
        }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var @params = new CancelParams() { Id = Guid.NewGuid() };
            var notification = new Notification("$/cancelRequest", JObject.Parse(JsonConvert.SerializeObject(@params)));

            mediator.HandleNotification(notification);
        }

    }

    public class MediatorTests_NotificationHandler
    {
        [Method("exit")]
        interface IExitHandler : INotificationHandler { }

        [Fact]
        public async Task Test1()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var mediator = new Mediator(new HandlerResolver(typeof(HandlerResolverTests).GetTypeInfo().Assembly), serviceProvider);

            var notification = new Notification("$/cancelRequest", null);

            mediator.HandleNotification(notification);
        }

    }
}