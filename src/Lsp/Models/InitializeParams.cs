using System;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class InitializeParams
    {
        /// <summary>
        /// The process Id of the parent process that started
        /// the server. Is null if the process has not been started by another process.
        /// If the parent process is not alive then the server should exit (see exit notification) its process.
        /// </summary>
        public long? ProcessId { get; set; }

        /// <summary>
        /// The rootPath of the workspace. Is null
        /// if no folder is open.
        ///
        /// @deprecated in favour of rootUri.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RootPath
        {
            get { return RootUri?.AbsolutePath; }
            set { RootUri = value == null ? null : new Uri($"file://{value}"); }
        }

        /// <summary>
        /// The rootUri of the workspace. Is null if no
        /// folder is open. If both `rootPath` and `rootUri` are set
        /// `rootUri` wins.
        /// </summary>
        public Uri RootUri { get; set; }

        /// <summary>
        /// User provided initialization options.
        /// </summary>
        public object InitializationOptions { get; set; }

        /// <summary>
        /// The capabilities provided by the client (editor or tool)
        /// </summary>
        public ClientCapabilities Capabilities { get; set; }

        /// <summary>
        /// The initial trace setting. If omitted trace is disabled ('off').
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public InitializeTrace Trace { get; set; } = InitializeTrace.off;
    }


    // ReSharper disable InconsistentNaming
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InitializeTrace
    {
        off,
        messages,
        verbose
    }
    // ReSharper restore InconsistentNaming
}