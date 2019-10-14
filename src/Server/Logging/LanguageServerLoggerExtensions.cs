using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerLoggerExtensions
    {
        public static ILoggingBuilder AddLanguageServer(this ILoggingBuilder builder, LogLevel minLevel = LogLevel.Information)
        {
            builder.Services.AddSingleton<LanguageServerLoggerSettings>(_ => new LanguageServerLoggerSettings { MinimumLogLevel = minLevel });
            builder.Services.AddSingleton<ILoggerProvider, LanguageServerLoggerProvider>();

            return builder;
        }
    }
}
