// See https://github.com/JoshKeegan/xRetry

using System;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace TestingUtils
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a theory that should be run
    /// by the test runner up to MaxRetries times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("TestingUtils.RetryTheoryDiscoverer", "TestingUtils")]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryTheoryAttribute : RetryFactAttribute
    {
        /// <inheritdoc/>
        public RetryTheoryAttribute(int maxRetries = 3, int delayBetweenRetriesMs = 0)
            : base(maxRetries, delayBetweenRetriesMs) {  }
    }

    public static class NSubstituteExtensions
    {
        public static object Protected(this object target, string name, params object[] args)
        {
            var type = target.GetType();
            var method = type
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.Name == name && x.IsVirtual);
            return method.Invoke(target, args);
        }
    }
}
