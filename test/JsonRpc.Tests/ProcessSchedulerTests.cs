using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using Xunit.Abstractions;
using static Microsoft.Reactive.Testing.ReactiveTest;

namespace JsonRpc.Tests
{
    public class ProcessSchedulerTests
    {
        public ProcessSchedulerTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        private readonly ITestOutputHelper _testOutputHelper;

        private class AllRequestProcessTypes : TheoryData<RequestProcessType>
        {
            public AllRequestProcessTypes()
            {
                Add(RequestProcessType.Serial);
                Add(RequestProcessType.Parallel);
            }
        }

        [Theory]
        [ClassData(typeof(AllRequestProcessTypes))]
        public void ShouldScheduleCompletedTask(RequestProcessType type)
        {
            var testScheduler = new TestScheduler();
            var testObservable = testScheduler.CreateColdObservable(
                OnNext(Subscribed, Unit.Default),
                OnCompleted(Subscribed, Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);


            s.Add(type, "bogus", DoStuff(testObservable, testObserver));

            testScheduler.AdvanceTo(Subscribed / 2);

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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);


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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);

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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);

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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), true, null, testScheduler);

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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);


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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, 3, testScheduler);


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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), true, 3, testScheduler);

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

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);

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
                OnError(Subscribed, new NotSupportedException(), Unit.Default)
            );
            var testObserver = testScheduler.CreateObserver<Unit>();

            using var s = new ProcessScheduler(new TestLoggerFactory(_testOutputHelper), false, null, testScheduler);

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
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnError && x.Time == Subscribed * 2 && x.Value.Exception is NotSupportedException);
            messages.Should().Contain(x => x.Value.Kind == NotificationKind.OnNext && x.Time == Subscribed * 3);
        }

        private static SchedulerDelegate DoStuff(IObservable<Unit> testObservable, IObserver<Unit> testObserver) =>
            (contentModifiedToken, scheduler) => testObservable.Amb(contentModifiedToken).Do(testObserver);
    }
}
