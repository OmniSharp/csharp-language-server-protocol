using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{

    namespace Models
    {
        /// <summary>
        /// Params to show a document.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(WindowNames.ShowDocument, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
        [Capability(typeof(ShowDocumentClientCapabilities))]
        public class ShowDocumentParams : IRequest<ShowDocumentResult>
        {
            /// <summary>
            /// The document uri to show.
            /// </summary>
            public DocumentUri Uri { get; set; } = null!;

            /// <summary>
            /// Indicates to show the resource in an external program.
            /// To show for example `https://code.visualstudio.com/`
            /// in the default WEB browser set `external` to `true`.
            /// </summary>
            [Optional]
            public bool? External { get; set; }

            /// <summary>
            /// An optional property to indicate whether the editor
            /// showing the document should take focus or not.
            /// Clients might ignore this property if an external
            /// program is started.
            /// </summary>
            [Optional]
            public bool? TakeFocus { get; set; }

            /// <summary>
            /// An optional selection range if the document is a text
            /// document. Clients might ignore the property if an
            /// external program is started or the file is not a text
            /// file.
            /// </summary>
            [Optional]
            public Range? Selection { get; set; }
        }

        /// <summary>
        /// The result of an show document request.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public class ShowDocumentResult
        {
            /// <summary>
            /// A boolean indicating if the show was successful.
            /// </summary>
            public bool Success { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Capabilities specific to the showDocument request
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.Window), nameof(WindowClientCapabilities.ShowDocument))]
        public class ShowDocumentClientCapabilities: ICapability
        {
            /// <summary>
            /// Capabilities specific to the `MessageActionItem` type.
            /// </summary>
            [Optional]
            public bool Support { get; set; }
        }
    }

    namespace Window
    {
    }
}
