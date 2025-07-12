using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DryIoc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class AggregateSettlerTests
    {
        private readonly TestLoggerFactory _loggerFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public AggregateSettlerTests(ITestOutputHelper testOutputHelper)
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
            var (settler, _) = CreateSettlers(testScheduler, TimeSpan.FromTicks(20), TimeSpan.FromTicks(100));

            // simulate SettleNext
            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(121, Unit.Default),
                ReactiveTest.OnCompleted(121, Unit.Default)
            );
        }

        [Theory]
        [InlineData(SettlerType.Client)]
        [InlineData(SettlerType.Server)]
        public void Should_Timeout_If_A_Request_Takes_To_Long(SettlerType settlerType)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(200), TimeSpan.FromTicks(500));

            matcher.ScheduleAbsoluteStart(settlerType, 0);
            matcher.ScheduleAbsoluteEnd(settlerType, ReactiveTest.Disposed);

            var observer = testScheduler.Start(() => settler.Settle(), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(601, Unit.Default),
                ReactiveTest.OnCompleted(802, Unit.Default)
            );
        }

        [Theory]
        [InlineData(SettlerType.Client)]
        [InlineData(SettlerType.Server)]
        public void Should_Wait_For_Request_To_Finish_And_Then_Wait(SettlerType settlerType)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            matcher.ScheduleRelativeStart(settlerType, 0);
            matcher.ScheduleRelativeEnd(settlerType, 300);

            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(401, Unit.Default),
                ReactiveTest.OnCompleted(401, Unit.Default)
            );
        }

        [Theory]
        [InlineData(SettlerType.Client, SettlerType.Client)]
        [InlineData(SettlerType.Server, SettlerType.Server)]
        public void Should_Wait_For_Subsequent_Requests_To_Finish_And_Then_Wait(SettlerType settlerTypeA, SettlerType settlerTypeB)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            matcher.ScheduleRelativeStart(settlerTypeA, 0);
            matcher.ScheduleRelativeEnd(settlerTypeA, 150);
            matcher.ScheduleRelativeStart(settlerTypeA, 200);
            matcher.ScheduleRelativeEnd(settlerTypeA, 400);
            matcher.ScheduleRelativeStart(settlerTypeB, 0);
            matcher.ScheduleRelativeEnd(settlerTypeB, 150);
            matcher.ScheduleRelativeStart(settlerTypeB, 200);
            matcher.ScheduleRelativeEnd(settlerTypeB, 400);

            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(502, Unit.Default),
                ReactiveTest.OnCompleted(502, Unit.Default)
            );
        }


        [Theory]
        [InlineData(SettlerType.Client, SettlerType.Server)]
        [InlineData(SettlerType.Server, SettlerType.Client)]
        public void Should_Wait_For_Subsequent_Requests_To_Finish_And_Then_Wait_On_Either_Side(SettlerType settlerTypeA, SettlerType settlerTypeB)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            matcher.ScheduleRelativeStart(settlerTypeA, 0);
            matcher.ScheduleRelativeEnd(settlerTypeA, 150);
            matcher.ScheduleRelativeStart(settlerTypeB, 200);
            matcher.ScheduleRelativeEnd(settlerTypeB, 400);

            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(251, Unit.Default),
                ReactiveTest.OnCompleted(251, Unit.Default)
            );
        }

        [Theory]
        [InlineData(SettlerType.Client, SettlerType.Client)]
        [InlineData(SettlerType.Server, SettlerType.Server)]
        public void Should_Wait_For_Overlapping_Requests_To_Finish_And_Then_Wait(SettlerType settlerTypeA, SettlerType settlerTypeB)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            matcher.ScheduleAbsoluteStart(settlerTypeA, 0);
            matcher.ScheduleAbsoluteStart(settlerTypeB, 200);
            matcher.ScheduleAbsoluteEnd(settlerTypeA, 250);
            matcher.ScheduleAbsoluteEnd(settlerTypeB, 350);

            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(451, Unit.Default),
                ReactiveTest.OnCompleted(451, Unit.Default)
            );
        }

        [Theory]
        [InlineData(SettlerType.Client, SettlerType.Server)]
        [InlineData(SettlerType.Server, SettlerType.Client)]
        public void Should_Wait_For_Overlapping_Requests_To_Finish_And_Then_Wait_On_Either_Side(SettlerType settlerTypeA, SettlerType settlerTypeB)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            matcher.ScheduleAbsoluteStart(settlerTypeA, 0);
            matcher.ScheduleAbsoluteStart(settlerTypeB, 200);
            matcher.ScheduleAbsoluteEnd(settlerTypeA, 250);
            matcher.ScheduleAbsoluteEnd(settlerTypeB, 350);

            var observer = testScheduler.Start(() => settler.Settle().Take(1), 100, 100, ReactiveTest.Disposed);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(351, Unit.Default),
                ReactiveTest.OnCompleted(351, Unit.Default)
            );
        }

        [Theory]
        [InlineData(SettlerType.Client, SettlerType.Client, SettlerType.Client)]
        [InlineData(SettlerType.Client, SettlerType.Server, SettlerType.Server)]
        [InlineData(SettlerType.Client, SettlerType.Client, SettlerType.Server)]
        [InlineData(SettlerType.Server, SettlerType.Server, SettlerType.Client)]
        [InlineData(SettlerType.Server, SettlerType.Client, SettlerType.Client)]
        [InlineData(SettlerType.Server, SettlerType.Server, SettlerType.Server)]
        public void Should_Complete_After_Final_Request_Timeout(SettlerType settlerTypeA, SettlerType settlerTypeB, SettlerType settlerTypeC)
        {
            var testScheduler = new TestScheduler();
            var (settler, matcher) = CreateSettlers(testScheduler, TimeSpan.FromTicks(100), TimeSpan.FromTicks(800));

            matcher.ScheduleAbsoluteStart(settlerTypeA, 0);
            matcher.ScheduleAbsoluteEnd(settlerTypeA, 200);
            matcher.ScheduleAbsoluteStart(settlerTypeB, 300);
            matcher.ScheduleAbsoluteEnd(settlerTypeB, 400);
            matcher.ScheduleAbsoluteStart(settlerTypeC, 500);
            matcher.ScheduleAbsoluteEnd(settlerTypeC, 550);

            var observer = testScheduler.Start(() => settler.Settle(), 100, 100, 2000);

            observer.Messages.Should().ContainInOrder(
                ReactiveTest.OnNext(301, Unit.Default),
                ReactiveTest.OnCompleted(1452, Unit.Default)
            );
        }

        private class AggregateRequestSettlerScheduler
        {
            private readonly TestScheduler _testScheduler;
            private readonly IRequestSettler _clientRequestSettler;
            private readonly IRequestSettler _serverRequestSettler;

            public AggregateRequestSettlerScheduler(TestScheduler testScheduler, IRequestSettler clientRequestSettler, IRequestSettler serverRequestSettler)
            {
                _testScheduler = testScheduler;
                _clientRequestSettler = clientRequestSettler;
                _serverRequestSettler = serverRequestSettler;
            }

            // ReSharper disable once UnusedMethodReturnValue.Local
            public IDisposable ScheduleAbsoluteStart(SettlerType settlerType, long dueTime) =>
                settlerType switch {
                    SettlerType.Client => _testScheduler.ScheduleAbsolute(dueTime, () => _clientRequestSettler.OnStartRequest()),
                    SettlerType.Server => _testScheduler.ScheduleAbsolute(dueTime, () => _serverRequestSettler.OnStartRequest()),
                    _                  => throw new NotImplementedException()
                };

            // ReSharper disable once UnusedMethodReturnValue.Local
            public IDisposable ScheduleAbsoluteEnd(SettlerType settlerType, long dueTime) =>
                settlerType switch {
                    SettlerType.Client => _testScheduler.ScheduleAbsolute(dueTime, () => _clientRequestSettler.OnEndRequest()),
                    SettlerType.Server => _testScheduler.ScheduleAbsolute(dueTime, () => _serverRequestSettler.OnEndRequest()),
                    _                  => throw new NotImplementedException()
                };

            // ReSharper disable once UnusedMethodReturnValue.Local
            public IDisposable ScheduleRelativeStart(SettlerType settlerType, long dueTime) =>
                settlerType switch {
                    SettlerType.Client => _testScheduler.ScheduleRelative(dueTime, () => _clientRequestSettler.OnStartRequest()),
                    SettlerType.Server => _testScheduler.ScheduleRelative(dueTime, () => _serverRequestSettler.OnStartRequest()),
                    _                  => throw new NotImplementedException()
                };

            // ReSharper disable once UnusedMethodReturnValue.Local
            public IDisposable ScheduleRelativeEnd(SettlerType settlerType, long dueTime) =>
                settlerType switch {
                    SettlerType.Client => _testScheduler.ScheduleRelative(dueTime, () => _clientRequestSettler.OnEndRequest()),
                    SettlerType.Server => _testScheduler.ScheduleRelative(dueTime, () => _serverRequestSettler.OnEndRequest()),
                    _                  => throw new NotImplementedException()
                };
        }

        private (ISettler settler, AggregateRequestSettlerScheduler matcher) CreateSettlers(
            TestScheduler scheduler, TimeSpan waitTime, TimeSpan timeout
        )
        {
            var container1 = CreateContainer(_loggerFactory);
            container1.RegisterMany<Settler>(
                Reuse.Singleton,
                Parameters.Of
                          .Type<JsonRpcTestOptions>(defaultValue: new JsonRpcTestOptions().WithWaitTime(waitTime).WithTimeout(timeout))
                          .Type<CancellationToken>(defaultValue: CancellationToken)
                          .Type<IScheduler>(defaultValue: scheduler)
            );
            var container2 = CreateContainer(_loggerFactory);
            container2.RegisterMany<Settler>(
                Reuse.Singleton,
                Parameters.Of
                          .Type<JsonRpcTestOptions>(defaultValue: new JsonRpcTestOptions().WithWaitTime(waitTime).WithTimeout(timeout))
                          .Type<CancellationToken>(defaultValue: CancellationToken)
                          .Type<IScheduler>(defaultValue: scheduler)
            );

            var settler = new AggregateSettler(container1.Resolve<ISettler>(), container2.Resolve<ISettler>());
            var clientSettler = container1.Resolve<IRequestSettler>();
            var serverSettler = container2.Resolve<IRequestSettler>();

            return ( settler, new AggregateRequestSettlerScheduler(scheduler, clientSettler, serverSettler) );
        }

        public enum SettlerType
        {
            Client,
            Server
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
