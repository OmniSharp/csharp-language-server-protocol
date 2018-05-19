using System;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class PublishDiagnosticsParams : IRequest
    {
        /// <summary>
        ///  The URI for which diagnostic information is reported.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        ///  An array of diagnostic information items.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }
    }
}
