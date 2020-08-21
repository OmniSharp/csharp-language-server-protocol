using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ICanBeIdentifiedHandler
    {
        /// <summary>
        /// An id that that determines if a handler is unique or not for purposes of routing requests
        /// </summary>
        /// <remarks>
        /// Some requests can "fan out" to multiple handlers to pull back data for the same document selector
        /// </remarks>
        Guid Id { get; }
    }
}
