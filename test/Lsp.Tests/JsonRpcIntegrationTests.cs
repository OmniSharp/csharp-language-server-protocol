// using System;
// using System.Linq;
// using System.Reactive.Subjects;
// using System.Threading;
// using System.Threading.Tasks;
// using FluentAssertions;
// using MediatR;
// using NSubstitute;
// using OmniSharp.Extensions.JsonRpc;
// using OmniSharp.Extensions.JsonRpc.Server;
// using OmniSharp.Extensions.JsonRpc.Testing;
// using OmniSharp.Extensions.LanguageProtocol.Testing;
// using Xunit;
// using Xunit.Abstractions;

// namespace Lsp.Tests
// {
//     public class JsonRpcIntegrationTests : LanguageProtocolTestBase
//     {
//         public JsonRpcIntegrationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
//         {
//         }

//         class Request : IRequest<Data>
//         {

//         }

//         class Data
//         {
//             public string Value { get; set; }
//         }

//         [Fact]
//         public async Task Should_Send_and_receive_requests()
//         {
//             var (client, server) = await Initialize(
//                 client => { client.OnRequest("myrequest", async () => new Data() {Value = "myresponse"}); },
//                 server => { server.OnRequest("myrequest", async () => new Data() {Value = string.Join("", "myresponse".Reverse())}); }
//             );

//             var serverResponse = await client.SendRequest("myrequest").Returning<Data>(CancellationToken);
//             serverResponse.Value.Should().Be("esnopserym");

//             var clientResponse = await server.SendRequest("myrequest").Returning<Data>(CancellationToken);
//             clientResponse.Value.Should().Be("myresponse");
//         }

//         [Fact]
//         public async Task Should_throw_when_sending_requests()
//         {
//             var (client, server) = await Initialize(
//                 client => { client.OnRequest("myrequest", async (Request request) => new Data() {Value = "myresponse"}); },
//                 server => { server.OnRequest("myrequest", async (Request request) => new Data() {Value = string.Join("", "myresponse".Reverse())}); }
//             );

//             Func<Task> clientRequest = () => client.SendRequest("myrequest", (Request)null).Returning<Data>(CancellationToken);
//             clientRequest.Should().Throw<InvalidParametersException>();

//             Func<Task> serverRequest = () => server.SendRequest("myrequest", (Request)null).Returning<Data>(CancellationToken);
//             serverRequest.Should().Throw<InvalidParametersException>();
//         }

//         [Fact]
//         public async Task Should_throw_when_receiving_requests()
//         {
//             var (client, server) = await Initialize(
//                 client => { client.OnRequest("myrequest", async (Request request) => (Data)null); },
//                 server => { server.OnRequest("myrequest", async (Request request) => (Data)null); }
//             );

//             Func<Task> clientRequest = () => client.SendRequest("myrequest", new Request()).Returning<Data>(CancellationToken);
//             clientRequest.Should().Throw<InternalErrorException>();

//             Func<Task> serverRequest = () => server.SendRequest("myrequest", new Request()).Returning<Data>(CancellationToken);
//             serverRequest.Should().Throw<InternalErrorException>();
//         }

//         [Fact]
//         public async Task Should_Send_and_receive_notifications()
//         {
//             var clientNotification = new AsyncSubject<Data>();
//             var serverNotification = new AsyncSubject<Data>();
//             var (client, server) = await Initialize(
//                 client => {
//                     client.OnNotification("mynotification", (Data data) => {
//                         clientNotification.OnNext(data);
//                         clientNotification.OnCompleted();
//                     });
//                 },
//                 server => {
//                     server.OnNotification("mynotification", (Data data) => {
//                         serverNotification.OnNext(data);
//                         serverNotification.OnCompleted();
//                     });
//                 }
//             );

//             client.SendNotification("mynotification", new Data() {Value = "myresponse"});
//             var serverResponse = await serverNotification;
//             serverResponse.Value.Should().Be("myresponse");

//             server.SendNotification("mynotification", new Data() {Value = string.Join("", "myresponse".Reverse())});
//             var clientResponse = await clientNotification;
//             clientResponse.Value.Should().Be("esnopserym");
//         }

//         [Fact]
//         public async Task Should_Send_and_cancel_requests_immediate()
//         {
//             var (client, server) = await Initialize(
//                 client => {
//                     client.OnRequest("myrequest", async (ct) => {
//                         await Task.Delay(TimeSpan.FromMinutes(1), ct);
//                         return new Data() {Value = "myresponse"};
//                     });
//                 },
//                 server => {
//                     server.OnRequest("myrequest", async (ct) => {
//                         await Task.Delay(TimeSpan.FromMinutes(1), ct);
//                         return new Data() {Value = string.Join("", "myresponse".Reverse())};
//                     });
//                 }
//             );

//             var cts = new CancellationTokenSource();
//             cts.Cancel();

//             {
//                 Func<Task> action = () => client.SendRequest("myrequest").Returning<Data>(cts.Token);
//                 await action.Should().ThrowAsync<OperationCanceledException>();
//             }

//             {
//                 Func<Task> action = () => server.SendRequest("myrequest").Returning<Data>(cts.Token);
//                 await action.Should().ThrowAsync<OperationCanceledException>();
//             }
//         }

//         [Fact]
//         public async Task Should_Send_and_cancel_requests_from_otherside()
//         {
//             var (client, server) = await Initialize(
//                 client => {
//                     client.OnRequest("myrequest", async (ct) => {
//                         await Task.Delay(TimeSpan.FromMinutes(1), ct);
//                         return new Data() {Value = "myresponse"};
//                     });
//                 },
//                 server => {
//                     server.OnRequest("myrequest", async (ct) => {
//                         await Task.Delay(TimeSpan.FromMinutes(1), ct);
//                         return new Data() {Value = string.Join("", "myresponse".Reverse())};
//                     });
//                 }
//             );

//             {
//                 var cts = new CancellationTokenSource();
//                 Func<Task> action = () => client.SendRequest("myrequest").Returning<Data>(cts.Token);
//                 cts.CancelAfter(10);
//                 await action.Should().ThrowAsync<RequestCancelledException>();
//             }

//             {
//                 var cts = new CancellationTokenSource();
//                 Func<Task> action = () => server.SendRequest("myrequest").Returning<Data>(cts.Token);
//                 cts.CancelAfter(10);
//                 await action.Should().ThrowAsync<RequestCancelledException>();
//             }
//         }

//         [Fact]
//         public async Task Should_Cancel_Parallel_Requests_When_Options_Are_Given()
//         {
//             var (client, server) = await Initialize(
//                 client => {
//                     client.OnRequest(
//                         "parallelrequest",
//                         async (ct) => {
//                             await Task.Delay(TimeSpan.FromSeconds(10), ct);
//                             return new Data() {Value = "myresponse"};
//                         },
//                         new JsonRpcHandlerOptions() {RequestProcessType = RequestProcessType.Parallel});
//                     client.OnRequest(
//                         "serialrequest",
//                         async (ct) => new Data() {Value = "myresponse"},
//                         new JsonRpcHandlerOptions() {RequestProcessType = RequestProcessType.Serial}
//                     );
//                 },
//                 server => {
//                     server.OnRequest(
//                         "parallelrequest",
//                         async (ct) => {
//                             await Task.Delay(TimeSpan.FromSeconds(10), ct);
//                             return new Data() {Value = "myresponse"};
//                         },
//                         new JsonRpcHandlerOptions() {RequestProcessType = RequestProcessType.Parallel});
//                     server.OnRequest(
//                         "serialrequest",
//                         async (ct) => new Data() {Value = "myresponse"},
//                         new JsonRpcHandlerOptions() {RequestProcessType = RequestProcessType.Serial}
//                     );
//                 }
//             );

//             {
//                 var task = client.SendRequest("parallelrequest").Returning<Data>(CancellationToken);
//                 await client.SendRequest("serialrequest").Returning<Data>(CancellationToken);
//                 Func<Task> action = () => task;
//                 await action.Should().ThrowAsync<ContentModifiedException>();
//             }

//             {
//                 var task = server.SendRequest("parallelrequest").Returning<Data>(CancellationToken);
//                 await server.SendRequest("serialrequest").Returning<Data>(CancellationToken);
//                 Func<Task> action = () => task;
//                 await action.Should().ThrowAsync<ContentModifiedException>();
//             }
//         }

//         [Fact]
//         public async Task Should_Link_Request_A_to_Request_B()
//         {
//             var (client, server) = await Initialize(
//                 client => {
//                     client
//                         .OnRequest("myrequest", async () => new Data() {Value = "myresponse"})
//                         .WithLink("myrequest", "myrequest2")
//                         ;
//                 },
//                 server => {
//                     server
//                         .OnRequest("myrequest", async () => new Data() {Value = string.Join("", "myresponse".Reverse())})
//                         .WithLink("myrequest", "myrequest2")
//                         ;
//                 }
//             );

//             var serverResponse = await client.SendRequest("myrequest2").Returning<Data>(CancellationToken);
//             serverResponse.Value.Should().Be("esnopserym");

//             var clientResponse = await server.SendRequest("myrequest2").Returning<Data>(CancellationToken);
//             clientResponse.Value.Should().Be("myresponse");
//         }
//     }
// }
