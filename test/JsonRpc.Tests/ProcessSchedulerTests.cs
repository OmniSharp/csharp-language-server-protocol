using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;

namespace JsonRpc.Tests
{
    public class ProcessSchedulerTests
    {
        private const int SLEEPTIME_MS = 50;

        class AllRequestProcessTypes : TheoryData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var values = (object[])Enum.GetValues(typeof(RequestProcessType));
                var qy = from v in values select new object[] { v };
                return qy.GetEnumerator();
            }
        }

        [Theory, ClassData(typeof(AllRequestProcessTypes))]
        public void ShouldScheduleCompletedTask(RequestProcessType type)
        {
            using (IScheduler s = new ProcessScheduler())
            {
                var done = false;
                s.Start();
                s.Add(type, () => {
                    done = true;
                    return Task.CompletedTask;
                });
                Thread.Sleep(SLEEPTIME_MS);
                done.Should().Be(true);
            }
        }

        [Theory, ClassData(typeof(AllRequestProcessTypes))]
        public void ShouldScheduleAwaitableTask(RequestProcessType type)
        {
            using (IScheduler s = new ProcessScheduler())
            {
                var done = false;
                s.Start();
                s.Add(RequestProcessType.Serial, async () => {
                    done = true;
                    await Task.Yield();
                });
                Thread.Sleep(SLEEPTIME_MS);
                done.Should().Be(true);
            }
        }

        [Fact]
        public void ShouldScheduleSerialInOrder()
        {
            using (IScheduler s = new ProcessScheduler())
            {
                var done = 0;
                var peek = 0;
                var peekWasMoreThanOne = 0;

                Func<Task> HandlePeek = async () => {
                    Interlocked.Increment(ref done); // record that I was called
                    var p = Interlocked.Increment(ref peek);
                    if (p > 1)
                        Interlocked.Increment(ref peekWasMoreThanOne);
                    await Task.Delay(SLEEPTIME_MS); // give a different HandlePeek task a chance to run
                    Interlocked.Decrement(ref peek);
                };

                s.Start();
                s.Add(RequestProcessType.Serial, HandlePeek);
                s.Add(RequestProcessType.Serial, HandlePeek);

                Thread.Sleep(SLEEPTIME_MS * 3);
                done.Should().Be(2);
                peek.Should().Be(0);
                peekWasMoreThanOne.Should().Be(0);
            }
        }

        [Fact]
        public void ShouldScheduleParallelInParallel()
        {
            using (IScheduler s = new ProcessScheduler())
            {
                var done = 0;
                var peek = 0;
                var peekWasMoreThanOne = 0;

                Func<Task> HandlePeek = async () => {
                    Interlocked.Increment(ref done); // record that I was called
                    var p = Interlocked.Increment(ref peek);
                    if (p > 1)
                        Interlocked.Increment(ref peekWasMoreThanOne);
                    await Task.Delay(SLEEPTIME_MS); // give a different HandlePeek task a chance to run
                    Interlocked.Decrement(ref peek);
                };

                s.Start();
                s.Add(RequestProcessType.Parallel, HandlePeek);
                s.Add(RequestProcessType.Parallel, HandlePeek);
                s.Add(RequestProcessType.Parallel, HandlePeek);
                s.Add(RequestProcessType.Parallel, HandlePeek);

                Thread.Sleep(SLEEPTIME_MS * 2);
                done.Should().Be(4);
                peek.Should().Be(0);
                peekWasMoreThanOne.Should().BeGreaterThan(0);
            }
        }
    }
}