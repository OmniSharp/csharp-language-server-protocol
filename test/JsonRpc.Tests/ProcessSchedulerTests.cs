using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
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

        class AllRequestProcessTypes : TheoryData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {RequestProcessType.Serial};
                yield return new object[] {RequestProcessType.Parallel};
            }
        }

        [Theory, ClassData(typeof(AllRequestProcessTypes))]
        public void ShouldScheduleCompletedTask(RequestProcessType type)
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);


            s.Add(type, "bogus", testObservable.Do(testObserver));

            testScheduler.AdvanceTo(50);

            testObservable.Subscriptions.Count.Should().Be(1);

            testScheduler.AdvanceTo(101);

            testObservable.Subscriptions.Count.Should().Be(1);
            testObserver.Messages.Should().Contain(z => z.Value.Kind == NotificationKind.OnNext);
            testObserver.Messages.Should().Contain(z => z.Value.Kind == NotificationKind.OnCompleted);
        }

        [Fact]
        public void ShouldScheduleSerialInOrder()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);


            for (var i = 0; i < 8; i++)
                s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(100, Unit.Default),
                    OnNext(200, Unit.Default),
                    OnNext(300, Unit.Default),
                    OnNext(400, Unit.Default),
                    OnNext(500, Unit.Default),
                    OnNext(600, Unit.Default),
                    OnNext(700, Unit.Default),
                    OnNext(800, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleParallelInParallel()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);

            for (var i = 0; i < 8; i++)
                s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleMixed()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);


            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(200, Unit.Default),
                    OnNext(300, Unit.Default),
                    OnNext(300, Unit.Default),
                    OnNext(400, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleSerial()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);


            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(4);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(100, Unit.Default),
                    OnNext(200, Unit.Default),
                    OnNext(300, Unit.Default),
                    OnNext(400, Unit.Default)
                );
        }

        [Fact]
        public void ShouldScheduleWithConcurrency()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), 3, scheduler);


            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Parallel, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(8);
            testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted).Should()
                .ContainInOrder(
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(100, Unit.Default),
                    OnNext(200, Unit.Default),
                    OnNext(300, Unit.Default),
                    OnNext(400, Unit.Default),
                    OnNext(400, Unit.Default),
                    OnNext(500, Unit.Default)
                );
        }

        [Fact]
        public void Should_Handle_Cancelled_Tasks()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var errorObservable = testScheduler.CreateColdObservable(
                OnError(100, new TaskCanceledException(), Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);

            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "somethingelse", errorObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(2);
            var messages = testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted)
                .ToArray();

            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == 100);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnError && x.Time == 200 && x.Value.Exception is OperationCanceledException);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == 300);
        }

        [Fact]
        public void Should_Handle_Exceptions_Tasks()
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(100, Unit.Default),
                OnCompleted(100, Unit.Default)
            );
            var errorObservable = testScheduler.CreateColdObservable(
                OnError(100, new NotSameException(), Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            var scheduler = new TestScheduler();
            scheduler.Start();
            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), null, scheduler);

            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "somethingelse", errorObservable.Do(testObserver));
            s.Add(RequestProcessType.Serial, "bogus", testObservable.Do(testObserver));

            testScheduler.Start();

            testObservable.Subscriptions.Count.Should().Be(2);
            var messages = testObserver.Messages
                .Where(z => z.Value.Kind != NotificationKind.OnCompleted)
                .ToArray();

            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == 100);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnError && x.Time == 200 && x.Value.Exception is NotSameException);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == 300);

        }
    }
}
