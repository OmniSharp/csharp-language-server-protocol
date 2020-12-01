using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class IntegrationTests : JsonRpcServerTestBase
    {
        public IntegrationTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        private class Request : IRequest<Data>
        {
        }

        private class Data
        {
            public string Value { get; set; } = null!;
        }

        [RetryFact]
        public async Task Should_Send_and_receive_requests()
        {
            var (client, server) = await Initialize(
                clientOptions => { clientOptions.OnRequest("myrequest", async () => new Data { Value = "myresponse" }); },
                serverOptions => { serverOptions.OnRequest("myrequest", async () => new Data { Value = string.Join("", "myresponse".Reverse()) }); }
            );

            var serverResponse = await client.SendRequest("myrequest").Returning<Data>(CancellationToken);
            serverResponse.Value.Should().Be("esnopserym");

            var clientResponse = await server.SendRequest("myrequest").Returning<Data>(CancellationToken);
            clientResponse.Value.Should().Be("myresponse");
        }

        [RetryFact]
        public async Task Should_throw_when_sending_requests()
        {
            var (client, server) = await Initialize(
                clientOptions => { clientOptions.OnRequest("myrequest", async (Request request) => new Data { Value = "myresponse" }); },
                serverOptions => { serverOptions.OnRequest("myrequest", async (Request request) => new Data { Value = string.Join("", "myresponse".Reverse()) }); }
            );

            Func<Task> clientRequest = () => client.SendRequest("myrequest", (Request) null!).Returning<Data>(CancellationToken);
            clientRequest.Should().Throw<InvalidParametersException>();

            Func<Task> serverRequest = () => server.SendRequest("myrequest", (Request) null!).Returning<Data>(CancellationToken);
            serverRequest.Should().Throw<InvalidParametersException>();
        }

        [RetryFact]
        public async Task Should_throw_when_receiving_requests()
        {
            var (client, server) = await Initialize(
                clientOptions => { clientOptions.OnRequest("myrequest", async (Request request) => (Data) null!); },
                serverOptions => { serverOptions.OnRequest("myrequest", async (Request request) => (Data) null!); }
            );

            Func<Task> clientRequest = () => client.SendRequest("myrequest", new Request()).Returning<Data>(CancellationToken);
            clientRequest.Should().Throw<InternalErrorException>();

            Func<Task> serverRequest = () => server.SendRequest("myrequest", new Request()).Returning<Data>(CancellationToken);
            serverRequest.Should().Throw<InternalErrorException>();
        }

        [RetryFact]
        public async Task Should_Send_and_receive_notifications()
        {
            var clientNotification = new AsyncSubject<Data>();
            var serverNotification = new AsyncSubject<Data>();
            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions.OnNotification(
                        "mynotification", (Data data) => {
                            clientNotification.OnNext(data);
                            clientNotification.OnCompleted();
                        }
                    );
                },
                serverOptions => {
                    serverOptions.OnNotification(
                        "mynotification", (Data data) => {
                            serverNotification.OnNext(data);
                            serverNotification.OnCompleted();
                        }
                    );
                }
            );

            client.SendNotification("mynotification", new Data { Value = "myresponse" });
            var serverResponse = await serverNotification;
            serverResponse.Value.Should().Be("myresponse");

            server.SendNotification("mynotification", new Data { Value = string.Join("", "myresponse".Reverse()) });
            var clientResponse = await clientNotification;
            clientResponse.Value.Should().Be("esnopserym");
        }

        [RetryFact]
        public async Task Should_Send_and_cancel_requests_immediate()
        {
            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions.OnRequest(
                        "myrequest", async ct => {
                            await Task.Delay(TimeSpan.FromMinutes(1), ct);
                            return new Data { Value = "myresponse" };
                        }
                    );
                },
                serverOptions => {
                    serverOptions.OnRequest(
                        "myrequest", async ct => {
                            await Task.Delay(TimeSpan.FromMinutes(1), ct);
                            return new Data { Value = string.Join("", "myresponse".Reverse()) };
                        }
                    );
                }
            );

            var cts = new CancellationTokenSource();
            cts.Cancel();

            {
                Func<Task> action = () => client.SendRequest("myrequest").Returning<Data>(cts.Token);
                await action.Should().ThrowAsync<OperationCanceledException>();
            }

            {
                Func<Task> action = () => server.SendRequest("myrequest").Returning<Data>(cts.Token);
                await action.Should().ThrowAsync<OperationCanceledException>();
            }
        }

        [RetryFact]
        public async Task Should_Send_and_cancel_requests_from_otherside()
        {
            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions.OnRequest(
                        "myrequest", async ct => {
                            await Task.Delay(TimeSpan.FromMinutes(1), ct);
                            return new Data { Value = "myresponse" };
                        }
                    );
                },
                serverOptions => {
                    serverOptions.OnRequest(
                        "myrequest", async ct => {
                            await Task.Delay(TimeSpan.FromMinutes(1), ct);
                            return new Data { Value = string.Join("", "myresponse".Reverse()) };
                        }
                    );
                }
            );

            {
                var cts = new CancellationTokenSource();
                Func<Task> action = () => client.SendRequest("myrequest").Returning<Data>(cts.Token);
                cts.CancelAfter(10);
                await action.Should().ThrowAsync<RequestCancelledException>();
            }

            {
                var cts = new CancellationTokenSource();
                Func<Task> action = () => server.SendRequest("myrequest").Returning<Data>(cts.Token);
                cts.CancelAfter(10);
                await action.Should().ThrowAsync<RequestCancelledException>();
            }
        }

        [RetryFact]
        public async Task Should_Cancel_Parallel_Requests_When_Options_Are_Given()
        {
            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions.OnRequest(
                        "parallelrequest",
                        async ct => {
                            await Task.Delay(TimeSpan.FromSeconds(10), ct);
                            return new Data { Value = "myresponse" };
                        },
                        new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Parallel }
                    );
                    clientOptions.OnRequest(
                        "serialrequest",
                        async ct => new Data { Value = "myresponse" },
                        new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial }
                    );
                },
                serverOptions => {
                    serverOptions.OnRequest(
                        "parallelrequest",
                        async ct => {
                            await Task.Delay(TimeSpan.FromSeconds(10), ct);
                            return new Data { Value = "myresponse" };
                        },
                        new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Parallel }
                    );
                    serverOptions.OnRequest(
                        "serialrequest",
                        async ct => new Data { Value = "myresponse" },
                        new JsonRpcHandlerOptions { RequestProcessType = RequestProcessType.Serial }
                    );
                }
            );

            {
                var task = client.SendRequest("parallelrequest").Returning<Data>(CancellationToken);
                await client.SendRequest("serialrequest").Returning<Data>(CancellationToken);
                Func<Task> action = () => task;
                await action.Should().ThrowAsync<ContentModifiedException>();
            }

            {
                var task = server.SendRequest("parallelrequest").Returning<Data>(CancellationToken);
                await server.SendRequest("serialrequest").Returning<Data>(CancellationToken);
                Func<Task> action = () => task;
                await action.Should().ThrowAsync<ContentModifiedException>();
            }
        }

        [RetryFact]
        public async Task Should_Link_Request_A_to_Request_B()
        {
            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions
                       .OnRequest("myrequest", async () => new Data { Value = "myresponse" })
                       .WithLink("myrequest", "myrequest2")
                        ;
                },
                serverOptions => {
                    serverOptions
                       .OnRequest("myrequest", async () => new Data { Value = string.Join("", "myresponse".Reverse()) })
                       .WithLink("myrequest", "myrequest2")
                        ;
                }
            );

            var serverResponse = await client.SendRequest("myrequest2").Returning<Data>(CancellationToken);
            serverResponse.Value.Should().Be("esnopserym");

            var clientResponse = await server.SendRequest("myrequest2").Returning<Data>(CancellationToken);
            clientResponse.Value.Should().Be("myresponse");
        }
    }
}
