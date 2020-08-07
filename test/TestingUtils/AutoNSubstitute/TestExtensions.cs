﻿using System.Threading;
using OmniSharp.Extensions.JsonRpc.Testing;
using Serilog.Events;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    public static class TestExtensions
    {
        public static void Wait(this CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Token.WaitHandle.WaitOne();
        }

        public static JsonRpcTestOptions ConfigureForXUnit(
            this JsonRpcTestOptions jsonRpcTestOptions, 
            ITestOutputHelper outputHelper, 
            LogEventLevel logEventLevel = LogEventLevel.Debug)
        {
            return jsonRpcTestOptions
                .WithClientLoggerFactory(new TestLoggerFactory(outputHelper, "{Timestamp:yyyy-MM-dd HH:mm:ss} [Client] [{Level}] {Message}{NewLine}{Exception}", logEventLevel))
                .WithServerLoggerFactory(new TestLoggerFactory(outputHelper, "{Timestamp:yyyy-MM-dd HH:mm:ss} [Server] [{Level}] {Message}{NewLine}{Exception}", logEventLevel));
        }
    }
}
