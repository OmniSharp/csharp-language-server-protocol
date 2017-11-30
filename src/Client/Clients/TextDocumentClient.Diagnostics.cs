using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client.Clients
{
    /// <summary>
    ///     Client for the LSP Text Document API.
    /// </summary>
    public partial class TextDocumentClient
    {
        /// <summary>
        ///     Register a handler for diagnostics published by the language server.
        /// </summary>
        /// <param name="handler">
        ///     A <see cref="PublishDiagnosticsHandler"/> that is called to publish the diagnostics.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        /// <remarks>
        ///     The diagnostics should replace any previously published diagnostics for the specified document.
        /// </remarks>
        public IDisposable OnPublishDiagnostics(PublishDiagnosticsHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return Client.HandleNotification<PublishDiagnosticsParams>(DocumentNames.PublishDiagnostics, notification =>
            {
                if (notification.Diagnostics == null)
                if (notification.Diagnostics == null)
                    return; // Invalid notification.

                var diagnostics = new List<Diagnostic>();
                if (notification.Diagnostics != null)
                    diagnostics.AddRange(notification.Diagnostics);

                handler(notification.Uri, diagnostics);
            });
        }
    }
}
