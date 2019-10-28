using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LanguageServerLoggerExtensions
    {
        public static ILoggingBuilder AddLanguageServer(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<LanguageServerLoggerSettings>(services => {
                var filterOptions = services.GetService<IOptions<LoggerFilterOptions>>();

                return new LanguageServerLoggerSettings { MinimumLogLevel = filterOptions.Value.MinLevel };
            });

            builder.Services.AddSingleton<ILoggerProvider, LanguageServerLoggerProvider>();

            return builder;
        }

        public static ILoggingBuilder AddLanguageServer(this ILoggingBuilder builder, LogLevel minLevel)
        {
            builder.Services.AddSingleton<LanguageServerLoggerSettings>(_ => new LanguageServerLoggerSettings { MinimumLogLevel = minLevel });
            builder.Services.AddSingleton<ILoggerProvider, LanguageServerLoggerProvider>();

            return builder;
        }
    }
}
