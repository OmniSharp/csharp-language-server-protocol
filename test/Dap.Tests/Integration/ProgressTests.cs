using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Dap.Tests.Integration.Fixtures;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
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

        [Fact]
        public async Task Should_Support_Progress_From_Sever_To_Client()
        {
            var data = new List<ProgressEvent>();
            Client.ProgressManager.Progress.Take(1).Switch().Subscribe(x => data.Add(x));

            using var workDoneObserver = Server.ProgressManager.Create(
                new ProgressStartEvent {
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

            await SettleNext();

            var results = data.Select(
                z => z switch {
                    ProgressStartEvent begin  => begin.Message,
                    ProgressUpdateEvent begin => begin.Message,
                    ProgressEndEvent begin    => begin.Message,
                }
            );

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Cancelling_Progress_From_Server_To_Client_Request()
        {

            var data = new List<ProgressEvent>();
            var sub = Client.ProgressManager.Progress.Take(1).Switch().Subscribe(x => data.Add(x));

            using var workDoneObserver = Server.ProgressManager.Create(
                new ProgressStartEvent {
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

            await SettleNext();

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(
                z => z switch {
                    ProgressStartEvent begin  => begin.Message,
                    ProgressUpdateEvent begin => begin.Message,
                    ProgressEndEvent begin    => begin.Message,
                }
            );

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2");
        }
    }
}
