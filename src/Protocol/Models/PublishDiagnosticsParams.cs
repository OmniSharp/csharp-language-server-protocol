using System;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.PublishDiagnostics)]
    public class PublishDiagnosticsParams : IRequest
    {
        /// <summary>
        ///  The URI for which diagnostic information is reported.
        /// </summary>
        [JsonConverter(typeof(AbsoluteUriConverter))]
        public Uri Uri { get; set; }

        /// <summary>
        /// Optional the version number of the document the diagnostics are published for.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public long Version { get; set; }

        /// <summary>
        ///  An array of diagnostic information items.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }
    }
}
