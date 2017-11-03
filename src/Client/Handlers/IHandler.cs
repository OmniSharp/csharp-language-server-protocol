namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     Represents a client-side message handler.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        ///     The name of the method handled by the handler.
        /// </summary>
        string Method { get; }

        /// <summary>
        ///     The kind of handler.
        /// </summary>
        HandlerKind Kind { get; }
    }
}
