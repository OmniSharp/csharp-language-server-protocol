using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Dap.Tests.Integration.Fixtures;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class ProgressTests : DebugAdapterProtocolFixtureTest<DefaultOptions, DefaultClient, DefaultServer>
    {

        public ProgressTests(ITestOutputHelper testOutputHelper, DebugAdapterProtocolFixture<DefaultOptions, DefaultClient, DefaultServer> fixture) : base(testOutputHelper, fixture)
        {
        }

        [Fact(Skip = "This api needs to be updated on the client implementation")]
        public async Task Should_Support_Progress_From_Sever_To_Client()
        {
            var id = new ProgressToken(Guid.NewGuid().ToString());
            var obs = Client.ProgressManager.Progress
                            .Where(z => z.ProgressToken == id)
                            .Take(1).Merge()
                            .Select(
                                 z => z switch {
                                     ProgressStartEvent begin  => begin.Message,
                                     ProgressUpdateEvent begin => begin.Message,
                                     ProgressEndEvent begin    => begin.Message,
                                 }
                             )
                            .ToArray()
                            .Replay();
            using var sub = obs.Connect();

            using var workDoneObserver = Server.ProgressManager.Create(
                new ProgressStartEvent {
                    ProgressId = id,
                    Cancellable = true,
                    Message = "Begin",
                    Percentage = 0,
                    Title = "Work is pending"
                }, onComplete: () => new ProgressEndEvent {
                    Message = "End"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 10,
                    Message = "Report 1"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 20,
                    Message = "Report 2"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 30,
                    Message = "Report 3"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 40,
                    Message = "Report 4"
                }
            );

            workDoneObserver.OnCompleted();

            var results = await obs.ToTask(CancellationToken);
            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact(Skip = "This api needs to be updated on the client implementation")]
        public async Task Should_Support_Cancelling_Progress_From_Server_To_Client_Request()
        {
            var id = new ProgressToken(Guid.NewGuid().ToString());
            var obs = Client.ProgressManager.Progress
                            .Where(z => z.ProgressToken == id)
                            .Take(1).Merge()
                            .Select(
                                 z => z switch {
                                     ProgressStartEvent begin  => begin.Message,
                                     ProgressUpdateEvent begin => begin.Message,
                                     ProgressEndEvent begin    => begin.Message,
                                 }
                             )
                            .ToArray()
                            .Replay();
            using var sub = obs.Connect();

            using var workDoneObserver = Server.ProgressManager.Create(
                new ProgressStartEvent {
                    ProgressId = id,
                    Cancellable = true,
                    Message = "Begin",
                    Percentage = 0,
                    Title = "Work is pending"
                }, onComplete: () => new ProgressEndEvent {
                    Message = "End"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 10,
                    Message = "Report 1"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 20,
                    Message = "Report 2"
                }
            );

            await SettleNext();

            sub.Dispose();

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 30,
                    Message = "Report 3"
                }
            );

            workDoneObserver.OnNext(
                new ProgressUpdateEvent {
                    Percentage = 40,
                    Message = "Report 4"
                }
            );

            workDoneObserver.OnCompleted();

            var results = await obs.ToTask(CancellationToken);
            results.Should().ContainInOrder("Begin", "Report 1", "Report 2");
        }
    }
}
