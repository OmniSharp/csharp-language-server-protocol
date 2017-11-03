using Serilog;
using System;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Logging
{
    /// <summary>
    ///     Extension methods for working with Serilog.
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        ///     Create a new logger that uses the specified type for "SourceContext".
        /// </summary>
        /// <typeparam name="TSource">
        ///     The source type.
        /// </typeparam>
        /// <param name="logger">
        ///     The base <see cref="ILogger"/>.
        /// </param>
        /// <returns>
        ///     The new <see cref="ILogger"/>.
        /// </returns>
        /// <remarks>
        ///     Unlike <see cref="Log.ForContext{TSource}"/>, this will overwrite the "SourceContext" property if it has already been set for the base logger.
        /// </remarks>
        public static ILogger ForSourceContext<TSource>(this ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            return logger.ForContext(typeof(TSource));
        }

        /// <summary>
        ///     Create a new logger that uses the full name of the specified type for "SourceContext".
        /// </summary>
        /// <param name="logger">
        ///     The base <see cref="ILogger"/>.
        /// </param>
        /// <param name="source">
        ///     The source type.
        /// </param>
        /// <returns>
        ///     The new <see cref="ILogger"/>.
        /// </returns>
        /// <remarks>
        ///     Unlike <see cref="Log.ForContext(Type)"/>, this will overwrite the "SourceContext" property if it has already been set for the base logger.
        /// </remarks>
        public static ILogger ForSourceContext(this ILogger logger, Type source)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return logger.ForSourceContext(source.FullName);
        }

        /// <summary>
        ///     Create a new logger that uses the specified value for "SourceContext".
        /// </summary>
        /// <param name="logger">
        ///     The base <see cref="ILogger"/>.
        /// </param>
        /// <param name="sourceContext">
        ///     The source context value.
        /// </param>
        /// <returns>
        ///     The new <see cref="ILogger"/>.
        /// </returns>
        /// <remarks>
        ///     Unlike <see cref="Log.ForContext(Type)"/>, this will overwrite the "SourceContext" property if it has already been set for the base logger.
        /// </remarks>
        public static ILogger ForSourceContext(this ILogger logger, string sourceContext)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (sourceContext == null)
                throw new ArgumentNullException(nameof(sourceContext));

            return logger.ForContext(
                new OverwriteSourceContextEnricher(sourceContext)
            );
        }
    }
}
