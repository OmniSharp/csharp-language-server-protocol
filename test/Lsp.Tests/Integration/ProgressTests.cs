﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Lsp.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class ProgressTests : LanguageProtocolFixtureTest<DefaultOptions, DefaultClient, DefaultServer>
    {
        public ProgressTests(ITestOutputHelper testOutputHelper, LanguageProtocolFixture<DefaultOptions, DefaultClient, DefaultServer> fixture) : base(testOutputHelper, fixture)
        {
        }

        private class Data
        {
            public string Value { get; set; } = "Value";
        }

        [Fact]
        public async Task Should_Send_Progress_From_Server_To_Client()
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());

            using var observer = Client.ProgressManager.For<Data>(token, CancellationToken);
            var workDoneObservable = Server.ProgressManager.Monitor(token, x => x.ToObject<Data>(Server.Services.GetRequiredService<ISerializer>().JsonSerializer));
            var observable = workDoneObservable.Replay();
            using var _ = observable.Connect();

            observer.OnNext(
                new Data {
                    Value = "1"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "3"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "2"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "4"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "5"
                }
            );

            await Observable.Create<Unit>(
                observer => new CompositeDisposable() {
                    observable.Select(z => z.Value).Take(5).Subscribe(_ => observer.OnNext(Unit.Default), observer.OnCompleted),
                    workDoneObservable
                }
            ).ToTask(CancellationToken);

            var data = await observable.Select(z => z.Value).ToArray().ToTask(CancellationToken);

            data.Should().ContainInOrder(new[] { "1", "3", "2", "4", "5" });
        }

        [Fact]
        public async Task Should_Send_Progress_From_Client_To_Server()
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());

            using var observer = Server.ProgressManager.For<Data>(token, CancellationToken);
            var workDoneObservable = Client.ProgressManager.Monitor(token, x => x.ToObject<Data>(Client.Services.GetRequiredService<ISerializer>().JsonSerializer));
            var observable = workDoneObservable.Replay();
            using var _ = observable.Connect();

            observer.OnNext(
                new Data {
                    Value = "1"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "3"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "2"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "4"
                }
            );
            observer.OnNext(
                new Data {
                    Value = "5"
                }
            );

            await Observable.Create<Unit>(
                observer => new CompositeDisposable() {
                    observable.Select(z => z.Value).Take(5).Subscribe(_ => observer.OnNext(Unit.Default), observer.OnCompleted),
                    workDoneObservable
                }
            ).ToTask(CancellationToken);

            var data = await observable.Select(z => z.Value).ToArray().ToTask(CancellationToken);

            data.Should().ContainInOrder(new[] { "1", "3", "2", "4", "5" });
        }

        [Fact]
        public void WorkDone_Should_Be_Supported()
        {
            Server.WorkDoneManager.IsSupported.Should().BeTrue();
            Client.WorkDoneManager.IsSupported.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Support_Creating_Work_Done_From_Sever_To_Client()
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var workDoneObservable = Client.WorkDoneManager.Monitor(token);
            var observable = workDoneObservable.Replay();
            using var _ = observable.Connect();

            using var workDoneObserver = await Server.WorkDoneManager.Create(
                token, new WorkDoneProgressBegin {
                    Cancellable = true,
                    Message = "Begin",
                    Percentage = 0,
                    Title = "Work is pending"
                }, onComplete: () => new WorkDoneProgressEnd {
                    Message = "End"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 10,
                    Message = "Report 1"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 20,
                    Message = "Report 2"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 30,
                    Message = "Report 3"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 40,
                    Message = "Report 4"
                }
            );

            workDoneObserver.OnCompleted();

            var results = await observable.Select(
                z => z switch {
                    WorkDoneProgressBegin begin  => begin.Message,
                    WorkDoneProgressReport begin => begin.Message,
                    WorkDoneProgressEnd begin    => begin.Message,
                }
            ).ToArray().ToTask(CancellationToken);

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Observing_Work_Done_From_Client_To_Server_Request()
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var workDoneObservable = Client.WorkDoneManager.Monitor(token);
            var observable = workDoneObservable.Replay();
            using var _ = observable.Connect();

            using var workDoneObserver = await Server.WorkDoneManager.Create(
                token, new WorkDoneProgressBegin {
                    Cancellable = true,
                    Message = "Begin",
                    Percentage = 0,
                    Title = "Work is pending"
                }, onComplete: () => new WorkDoneProgressEnd {
                    Message = "End"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 10,
                    Message = "Report 1"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 20,
                    Message = "Report 2"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 30,
                    Message = "Report 3"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 40,
                    Message = "Report 4"
                }
            );

            workDoneObserver.OnCompleted();

            var results = await observable.Select(
                z => z switch {
                    WorkDoneProgressBegin begin  => begin.Message,
                    WorkDoneProgressReport begin => begin.Message,
                    WorkDoneProgressEnd begin    => begin.Message,
                }
            ).ToArray().ToTask(CancellationToken);

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2", "Report 3", "Report 4", "End");
        }

        [Fact]
        public async Task Should_Support_Cancelling_Work_Done_From_Client_To_Server_Request()
        {
            var token = new ProgressToken(Guid.NewGuid().ToString());

            var workDoneObservable = Client.WorkDoneManager.Monitor(token);
            var observable = workDoneObservable.Replay();
            using var _ = observable.Connect();

            using var workDoneObserver = await Server.WorkDoneManager.Create(
                token, new WorkDoneProgressBegin {
                    Cancellable = true,
                    Message = "Begin",
                    Percentage = 0,
                    Title = "Work is pending"
                }, onComplete: () => new WorkDoneProgressEnd {
                    Message = "End"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 10,
                    Message = "Report 1"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 20,
                    Message = "Report 2"
                }
            );

            await observable.Take(3).ToTask(CancellationToken);

            workDoneObservable.Dispose();

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 30,
                    Message = "Report 3"
                }
            );

            workDoneObserver.OnNext(
                new WorkDoneProgressReport {
                    Percentage = 40,
                    Message = "Report 4"
                }
            );

            workDoneObserver.OnCompleted();

            var results = await observable.Select(
                z => z switch {
                    WorkDoneProgressBegin begin  => begin.Message,
                    WorkDoneProgressReport begin => begin.Message,
                    WorkDoneProgressEnd begin    => begin.Message,
                }
            ).ToArray().ToTask(CancellationToken);

            results.Should().ContainInOrder("Begin", "Report 1", "Report 2");
        }
    }
}
