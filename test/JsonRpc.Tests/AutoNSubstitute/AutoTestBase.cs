using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute.Internals;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    public abstract class AutoTestBase
    {
        private AutoSubstitute _autoSubstitute;
        private readonly Action<IContainer> _action;

        protected AutoTestBase(ITestOutputHelper testOutputHelper)
            : this(testOutputHelper, cb => { })
        {
        }

        protected AutoTestBase(ITestOutputHelper testOutputHelper, Action<IContainer> action)
        {
            LoggerFactory = new TestLoggerFactory(testOutputHelper);
            Logger = LoggerFactory.CreateLogger("default");
            _action = cb => {
                action(cb);
                Services
                    .AddLogging()
                    .AddSingleton(LoggerFactory);
                cb.Populate(Services);
            };
        }

        public ILoggerFactory LoggerFactory { get; }
        public ILogger Logger { get; }
        public IServiceCollection Services { get; } = new ServiceCollection();

        public AutoSubstitute AutoSubstitute => _autoSubstitute ??= new AutoSubstitute(new Container().WithDependencyInjectionAdapter(), _ => {
            _action(_);
            return _;
        });

        public IServiceProvider ServiceProvider => AutoSubstitute.Container.Resolve<IServiceProvider>();
    }
}
