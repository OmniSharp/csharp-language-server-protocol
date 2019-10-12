using System;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class PublishDiagnosticsParams : IRequest
    {
        /// <summary>
        ///  The URI for which diagnostic information is reported.
        /// </summary>
        [JsonConverter(typeof(AbsoluteUriConverter))]
        public Uri Uri { get; set; }

        /// <summary>
        ///  An array of diagnostic information items.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }
    }
}
