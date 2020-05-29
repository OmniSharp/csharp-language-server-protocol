using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Reactive.Testing;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit.Abstractions;
using Xunit.Sdk;
using static Microsoft.Reactive.Testing.ReactiveTest;

namespace JsonRpc.Tests
{
    public class ProcessSchedulerTests
    {
        public ProcessSchedulerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private readonly ITestOutputHelper _testOutputHelper;

        class AllRequestProcessTypes : TheoryData<RequestProcessType>
        {
            public AllRequestProcessTypes()
            {
                Add(RequestProcessType.Serial);
                Add(RequestProcessType.Parallel);
            }
        }

        [Theory, ClassData(typeof(AllRequestProcessTypes))]
        public void ShouldScheduleCompletedTask(RequestProcessType type)
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);


            s.Add(type, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.AdvanceTo(Subscribed/2);

            testObservable.Subscriptions.Count.Should().Be(1);

            testScheduler.AdvanceTo(Subscribed + 1);

            testObservable.Subscriptions.Count.Should().Be(1);
            testObserver.Messages.Should().Contain(z => z.Value.Kind == NotificationKind.OnNext);
            testObserver.Messages.Should().Contain(z => z.Value.Kind == NotificationKind.OnCompleted);
        }

        [Fact]
        public void ShouldScheduleSerialInOrder()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);


            for (var i = 0; i < 8; i++)
                s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default),
                    OnNext(Subscribed * 5, Unit.Default),
                    OnNext(Subscribed * 6, Unit.Default),
                    OnNext(Subscribed * 7, Unit.Default),
                    OnNext(Subscribed * 8, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleParallelInParallel()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);

            for (var i = 0; i < 8; i++)
                s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleMixed()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);

            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleMixed_WithContentModified()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), true, null, TimeSpan.FromSeconds(30), testScheduler);

            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(11);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleSerial()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);


            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(4);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleWithConcurrency()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, 3, TimeSpan.FromSeconds(30), testScheduler);


            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default),
                    OnNext(Subscribed * 5, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleWithConcurrency_WithContentModified()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), true, 3, TimeSpan.FromSeconds(30), testScheduler);

            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(11);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default),
                    OnNext(Subscribed * 3, Unit.Default)
                );
        }

        [Fact]
        public void Should_Handle_Cancelled_Tasks()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var errorObservable = testScheduler.CreateColdObservable(
                OnError(Subscribed, new TaskCanceledException(), Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);

            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "somethingelse", DoStuff(errorObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(2);
            errorObservable.Subscriptions.Should().HaveCount(1);
            var messages = testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted)
                .ToArray();

            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == Subscribed);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnError && x.Time == Subscribed * 2 && x.Value.Exception is OperationCanceledException);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == Subscribed * 3);
        }

        [Fact]
        public void Should_Handle_Exceptions_Tasks()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var errorObservable = testScheduler.CreateColdObservable(
                OnError(Subscribed, new NotSameException(), Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromSeconds(30), testScheduler);

            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "somethingelse", DoStuff(errorObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(2);
            errorObservable.Subscriptions.Should().HaveCount(1);
            var messages = testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted)
                .ToArray();

            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == Subscribed);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnError && x.Time == Subscribed * 2 && x.Value.Exception is NotSameException);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == Subscribed * 3);

        }

        [Fact]
        public void Should_Handle_Request_Cancellation()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();
            var disposedCount = 0;

            SchedulerDelegate Handle(IObservable<Unit> observable, IObserver<Unit> observer) {
                return contentModifiedToken => Observable.Create<Unit>(o => {
                    return new CompositeDisposable() {
                        Disposable.Create(() => Interlocked.Increment(ref disposedCount)),
                        observable.Amb(contentModifiedToken).Subscribe(o)
                    };
                }).Do(observer);
            }

            using (var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromTicks(Subscribed / 4), testScheduler))
            {
                s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver) );
                s.Add(RequestProcessType.Serial, "somethingelse", Handle(testObservable, testObserver) );
                s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver) );
                testScheduler.Start();
            }

            testObservable.Subscriptions.Should().HaveCount(3);
            testObserver.Messages.Should().BeEmpty();
            disposedCount.Should().Be(3);
        }

        [Fact]
        public void Should_Handle_Request_Cancellation_With_Errors()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var errorObservable = testScheduler.CreateColdObservable(
                OnError(Subscribed/2, new NotSameException(), Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();
            var disposedCount = 0;

            SchedulerDelegate Handle(IObservable<Unit> observable, IObserver<Unit> observer) {
                return contentModifiedToken => Observable.Create<Unit>(o => {
                    return new CompositeDisposable() {
                        Disposable.Create(() => Interlocked.Increment(ref disposedCount)),
                        observable.Amb(contentModifiedToken).Subscribe(o)
                    };
                }).Do(observer);
            }

            using (var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromTicks(Subscribed / 4), testScheduler))
            {
                s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver) );
                s.Add(RequestProcessType.Serial, "somethingelse", Handle(errorObservable, testObserver) );
                s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver) );
                testScheduler.Start();
            }

            testObservable.Subscriptions.Should().HaveCount(2);
            errorObservable.Subscriptions.Should().HaveCount(1);
            testObserver.Messages.Should().BeEmpty();
            disposedCount.Should().Be(3);
        }

        [Fact]
        public void Should_Handle_Request_Cancellation_With_Errors_WithConcurrency()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var willBeCancelledObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed * 2, Unit.Default),
                OnCompleted(Subscribed * 2, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var disposedCount = 0;
            SchedulerDelegate Handle(IObservable<Unit> observable, IObserver<Unit> observer) {
                return contentModifiedToken => Observable.Create<Unit>(o => {
                    var complete = false;
                    return new CompositeDisposable() {
                        observable.Amb(contentModifiedToken).Do(_ => { }, () => complete = true).Subscribe(o),
                        Disposable.Create(() => {
                            if (!complete)
                            {
                                Interlocked.Increment(ref disposedCount);
                            }
                        })
                    };
                }).Do(observer);
            }

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, TimeSpan.FromTicks(Subscribed), testScheduler);

            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(willBeCancelledObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(willBeCancelledObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", Handle(willBeCancelledObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Serial, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(willBeCancelledObservable, testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", Handle(testObservable, testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(12);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted)
                .Should()
                .ContainInOrder(
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed, Unit.Default),
                    OnNext(Subscribed * 2, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default),
                    OnNext(Subscribed * 4, Unit.Default),
                    OnNext(Subscribed * 5, Unit.Default),
                    OnNext(Subscribed * 6, Unit.Default),
                    OnNext(Subscribed * 7, Unit.Default),
                    OnNext(Subscribed * 7, Unit.Default)
                );
            disposedCount.Should().Be(4);
        }

        private static SchedulerDelegate DoStuff(IObservable<Unit> testObservable, IObserver<Unit> testObserver)
        {
            return contentModifiedToken => testObservable.Amb(contentModifiedToken).Do(testObserver);
        }
    }
}
