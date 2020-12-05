using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server.Logging;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerLoggerExtensions
    {
        public static ILoggingBuilder AddLanguageProtocolLogging(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, LanguageServerLoggerProvider>();
            return builder;
        }
    }
}
