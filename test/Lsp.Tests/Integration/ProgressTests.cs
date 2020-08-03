using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace Lsp.Tests.Integration
{
    public class ProgressTests : LanguageProtocolTestBase
    {
        public ProgressTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper))
        
        {
        }

        class Data
        {
            public string Value { get; set; } = "Value";
        }

        [Fact]
        public async Task Should_Send_Progress_From_Server_To_Client()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<string>();

            var observer = client.ProgressManager.For<Data>(token, CancellationToken);
            server.ProgressManager.Monitor(token, x => x.ToObject<Data>(server.GetRequiredService<ISerializer>().JsonSerializer)).Subscribe(x => data.Add(x.Value));

            observer.OnNext(new Data() {
                Value = "1"
            });
            observer.OnNext(new Data() {
                Value = "3"
            });
            observer.OnNext(new Data() {
                Value = "2"
            });
            observer.OnNext(new Data() {
                Value = "4"
            });
            observer.OnNext(new Data() {
                Value = "5"
            });
            observer.OnCompleted();

            await SettleNext();

            data.Should().ContainInOrder("1", "3", "2", "4", "5");
        }

        [Fact]
        public async Task Should_Send_Progress_From_Client_To_Server()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<string>();

            using var observer = server.ProgressManager.For<Data>(token, CancellationToken);
            client.ProgressManager.Monitor(token, x => x.ToObject<Data>(client.GetRequiredService<ISerializer>().JsonSerializer)).Subscribe(x => data.Add(x.Value));

            observer.OnNext(new Data() {
                Value = "1"
            });
            observer.OnNext(new Data() {
                Value = "3"
            });
            observer.OnNext(new Data() {
                Value = "2"
            });
            observer.OnNext(new Data() {
                Value = "4"
            });
            observer.OnNext(new Data() {
                Value = "5"
            });
            observer.OnCompleted();

            await SettleNext();

            data.Should().ContainInOrder("1", "3", "2", "4", "5");
        }

        [Fact]
        public async Task WorkDone_Should_Be_Supported()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            server.WorkDoneManager.IsSupported.Should().BeTrue();
            client.WorkDoneManager.IsSupported.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Support_Creating_Work_Done_From_Sever_To_Client()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<WorkDoneProgress>();
            using var workDoneObservable = client.WorkDoneManager.Monitor(token);
            workDoneObservable.Subscribe(x => data.Add(x));

            using var workDoneObserver = await server.WorkDoneManager.Create(token, new WorkDoneProgressBegin() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new WorkDoneProgressEnd() {
                Message = "End"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 20,
                Message = "Report 2"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                WorkDoneProgressBegin begin => begin.Message,
                WorkDoneProgressReport begin => begin.Message,
                WorkDoneProgressEnd begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Observing_Work_Done_From_Client_To_Server_Request()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<WorkDoneProgress>();
            using var workDoneObservable = client.WorkDoneManager.Monitor(token);
            workDoneObservable.Subscribe(x => data.Add(x));

            using var workDoneObserver = await server.WorkDoneManager.Create(token, new WorkDoneProgressBegin() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new WorkDoneProgressEnd() {
                Message = "End"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 20,
                Message = "Report 2"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                WorkDoneProgressBegin begin => begin.Message,
                WorkDoneProgressReport begin => begin.Message,
                WorkDoneProgressEnd begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Cancelling_Work_Done_From_Client_To_Server_Request()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var data = new List<WorkDoneProgress>();
            using var workDoneObservable = client.WorkDoneManager.Monitor(token);
            workDoneObservable.Subscribe(x => data.Add(x));

            using var workDoneObserver = await server.WorkDoneManager.Create(token, new WorkDoneProgressBegin() {
                Cancellable = true,
                Message = "Begin",
                Percentage = 0,
                Title = "Work is pending"
            }, onComplete: () => new WorkDoneProgressEnd() {
                Message = "End"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 10,
                Message = "Report 1"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 20,
                Message = "Report 2"
            });

            await SettleNext();
            workDoneObservable.Dispose();
            await SettleNext();

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 30,
                Message = "Report 3"
            });

            workDoneObserver.OnNext(new WorkDoneProgressReport() {
                Percentage = 40,
                Message = "Report 4"
            });

            workDoneObserver.OnCompleted();

            await SettleNext();

            var results = data.Select(z => z switch {
                WorkDoneProgressBegin begin => begin.Message,
                WorkDoneProgressReport begin => begin.Message,
                WorkDoneProgressEnd begin => begin.Message,
            });

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2");
        }

        private void ConfigureClient(LanguageClientOptions options)
        {

        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            // options.OnCodeLens()
        }
    }
}
