using Serilog.Core;
using System;
using Serilog.Events;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Logging
{
    /// <summary>
    ///     Serilog event enricher that always sets the "SourceContext" property in log messages (even if it is already present).
    /// </summary>
    class OverwriteSourceContextEnricher
        : ILogEventEnricher
    {
        /// <summary>
        ///     The "SourceContext" value to use.
        /// </summary>
        readonly string _sourceContext;

        /// <summary>
        ///     Create a new <see cref="OverwriteSourceContextEnricher"/>.
        /// </summary>
        /// <param name="sourceContext">
        ///     The "SourceContext" value to use.
        /// </param>
        public OverwriteSourceContextEnricher(string sourceContext)
        {
            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));

            _sourceContext = sourceContext;
        }

        /// <summary>
        ///     Enrich the specified log event.
        /// </summary>
        /// <param name="logEvent">
        ///     The target log event.
        /// </param>
        /// <param name="propertyFactory">
        ///     A log event property factory used to create the "SourceContext" property.
        /// </param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            if (propertyFactory == null)
                throw new ArgumentNullException(nameof(propertyFactory));

            LogEventProperty sourceContextProperty = propertyFactory.CreateProperty("SourceContext", _sourceContext);
            logEvent.AddOrUpdateProperty(sourceContextProperty);
        }
    }
}
