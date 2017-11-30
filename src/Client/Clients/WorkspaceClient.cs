using System;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Client.Clients
{
    /// <summary>
    ///     Client for the LSP Workspace API.
    /// </summary>
    public class WorkspaceClient
    {
        /// <summary>
        ///     Create a new <see cref="WorkspaceClient"/>.
        /// </summary>
        /// <param name="client">
        ///     The language client providing the API.
        /// </param>
        public WorkspaceClient(LanguageClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Client = client;
        }

        /// <summary>
        ///     The language client providing the API.
        /// </summary>
        public LanguageClient Client { get; }

        /// <summary>
        ///     Notify the language server that workspace configuration has changed.
        /// </summary>
        /// <param name="configuration">
        ///     A <see cref="JObject"/> representing the workspace configuration (or a subset thereof).
        /// </param>
        public void DidChangeConfiguration(JObject configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            Client.SendNotification("workspace/didChangeConfiguration", new JObject(
                new JProperty("settings", configuration)
            ));
        }
    }
}
