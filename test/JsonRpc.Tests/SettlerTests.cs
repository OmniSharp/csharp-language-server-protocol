using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class SettlerTests
    {
        private readonly TestLoggerFactory _loggerFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public SettlerTests(ITestOutputHelper testOutputHelper)
        {
            _loggerFactory = new TestLoggerFactory(testOutputHelper);
            _cancellationTokenSource = new CancellationTokenSource();
            if (!Debugger.IsAttached)
            {
                _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(60));
            }
        }

        private CancellationToken CancellationToken => _cancellationTokenSource.Token;

        [Fact]
        public void Should_Complete_If_There_Are_No_Pending_Requests()
        {
            var testScheduler = new TestScheduler();
            var (settler, _) = CreateSettler(testScheduler, TimeSpan.FromTicks(20), TimeSpan.FromTicks(100));

            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(121, Unit.Default),
                ReactiveTest.OnCompleted(121, Unit.Default)
            );
        }

        [Fact]
        public async Task Should_Complete_If_There_Are_No_Pending_Requests_SettleNext()
        {
            var testScheduler = new TestScheduler();
            var (settler, _) = CreateSettler(testScheduler, TimeSpan.FromTicks(20), TimeSpan.FromTicks(100));

            var observer = testScheduler.Start(() => settler.SettleNextInternal(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(121, Unit.Default),
                ReactiveTest.OnCompleted(121, Unit.Default)
            );
        }

        [Fact]
        public void Should_Timeout_If_A_Request_Takes_To_Long()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(200), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(ReactiveTest.Disposed, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.Settle(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(901, Unit.Default),
                ReactiveTest.OnCompleted(901, Unit.Default)
            );
        }

        [Fact]
        public void Should_Timeout_If_A_Request_Takes_To_Long_SettleNext()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(200), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(ReactiveTest.Disposed, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.SettleNextInternal(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(902, Unit.Default),
                ReactiveTest.OnCompleted(902, Unit.Default)
            );
        }

        [Fact]
        public void Should_Reset_Timeout_If_A_Request_Takes_A_Long_Time_SettleNext()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(200), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(700, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(600, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(1300, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.SettleNextInternal(), 100, 100, 1600);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(1501, Unit.Default),
                ReactiveTest.OnCompleted(1501, Unit.Default)
            );
        }

        [Fact]
        public void Should_Wait_For_Request_To_Finish_And_Then_Wait()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(300, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(401, Unit.Default),
                ReactiveTest.OnCompleted(401, Unit.Default)
            );
        }

        [Fact]
        public void Should_Wait_For_Request_To_Finish_And_Then_Wait_SettleNext()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(300, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.SettleNextInternal(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(401, Unit.Default),
                ReactiveTest.OnCompleted(401, Unit.Default)
            );
        }

        [Fact]
        public void Should_Wait_For_Subsequent_Requests_To_Finish_And_Then_Wait()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(150, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(200, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(400, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(501, Unit.Default),
                ReactiveTest.OnCompleted(501, Unit.Default)
            );
        }

        [Fact]
        public void Should_Wait_For_Subsequent_Requests_To_Finish_And_Then_Wait_SettleNext()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(150, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(200, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(400, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.SettleNextInternal(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(501, Unit.Default),
                ReactiveTest.OnCompleted(501, Unit.Default)
            );
        }

        [Fact]
        public void Should_Wait_For_Overlapping_Requests_To_Finish_And_Then_Wait()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(200, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(250, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(350, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(451, Unit.Default),
                ReactiveTest.OnCompleted(451, Unit.Default)
            );
        }

        [Fact]
        public void Should_Wait_For_Overlapping_Requests_To_Finish_And_Then_Wait_SettleNext()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(200, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(250, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(350, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.SettleNextInternal(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(451, Unit.Default),
                ReactiveTest.OnCompleted(451, Unit.Default)
            );
        }

        [Fact]
        public void Should_Complete_After_Final_Request_Timeout()
        {
            var testScheduler = new TestScheduler();
            var (settler, requestSettler) = CreateSettler(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            testScheduler.ScheduleAbsolute(0, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(200, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(300, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(400, () => requestSettler.OnEndRequest());
            testScheduler.ScheduleAbsolute(500, () => requestSettler.OnStartRequest());
            testScheduler.ScheduleAbsolute(550, () => requestSettler.OnEndRequest());
            var observer = testScheduler.Start(() => settler.Settle(), 100, 100, 3000);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(301, Unit.Default),
                ReactiveTest.OnNext(501, Unit.Default),
                ReactiveTest.OnNext(651, Unit.Default),
                ReactiveTest.OnNext(1452, Unit.Default),
                ReactiveTest.OnCompleted(1452, Unit.Default)
            );
        }

        private (Settler settler, IRequestSettler requestSettler) CreateSettler(TestScheduler scheduler, TimeSpan waitTime, TimeSpan timeout)
        {
            var container = CreateContainer(_loggerFactory);
            container.RegisterMany<Settler>(
                Reuse.Singleton,
                Parameters.Of
                          .Type<JsonRpcTestOptions>(defaultValue: new JsonRpcTestOptions().WithWaitTime(waitTime).WithTimeout(timeout))
                          .Type<CancellationToken>(defaultValue: CancellationToken)
                          .Type<IScheduler>(defaultValue: scheduler)
            );

            return ( container.Resolve<Settler>(), container.Resolve<IRequestSettler>() );
        }

        private static IContainer CreateContainer(ILoggerFactory loggerFactory)
        {
            var container = new Container()
                           .WithDependencyInjectionAdapter(new ServiceCollection().AddLogging())
                           .With(
                                rules => rules
                                        .WithResolveIEnumerableAsLazyEnumerable()
                                        .With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic)
                            );
            container.RegisterInstance(loggerFactory);

            return container;
        }
    }
}
