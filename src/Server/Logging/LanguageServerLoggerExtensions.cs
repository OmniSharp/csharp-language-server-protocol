using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
