using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    public class ProgressTests : DebugAdapterProtocolTestBase
    {
        public ProgressTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromSeconds(1))
            .WithSettleTimeout(TimeSpan.FromSeconds(2))
        )
        {
        }

        class Data
        {
            public string Value { get; set; } = "Value";
        }

        [Fact(Skip = "Test fails periodically on CI but not locally")]
        public async Task Should_Support_Progress_From_Sever_To_Client()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var data = new List<ProgressEvent>();
            client.ProgressManager.Progress.Take(1).Switch().Subscribe(x => data.Add(x));

            using var workDoneObserver = server.ProgressManager.Create(new ProgressStartEvent() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new ProgressEndEvent() {
                Message = "End"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 20,
                Message = "Report 2"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                ProgressStartEvent begin => begin.Message,
                ProgressUpdateEvent begin => begin.Message,
                ProgressEndEvent begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Cancelling_Progress_From_Server_To_Client_Request()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var data = new List<ProgressEvent>();
            var sub = client.ProgressManager.Progress.Take(1).Switch().Subscribe(x => data.Add(x));

            using var workDoneObserver = server.ProgressManager.Create(new ProgressStartEvent() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new ProgressEndEvent() {
                Message = "End"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 20,
                Message = "Report 2"
            });

            await SettleNext();

            sub.Dispose();

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new ProgressUpdateEvent() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                ProgressStartEvent begin => begin.Message,
                ProgressUpdateEvent begin => begin.Message,
                ProgressEndEvent begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2");
        }

        private void ConfigureClient(DebugAdapterClientOptions options)
        {

        }

        private void ConfigureServer(DebugAdapterServerOptions options)
        {
            // options.OnCodeLens()
        }
    }
}
