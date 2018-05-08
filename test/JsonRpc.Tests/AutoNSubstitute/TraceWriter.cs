using System;
using FluentAssertions.Equivalency;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    public class TraceWriter : ITraceWriter
    {
        private readonly ILogger _logger;

        public TraceWriter(ILogger logger)
        {
            _logger = logger;
        }

        public void AddSingle(string trace) => _logger.LogInformation(trace);

        public IDisposable AddBlock(string trace) => _logger.BeginScope(trace);
    }
}
