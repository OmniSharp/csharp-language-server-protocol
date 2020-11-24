using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// The show message request is sent from a server to a client to ask the client to display a particular message in the user interface. In addition to the show message notification
        /// the request allows to pass actions and to wait for an answer from the client.
        /// </summary>
        [Parallel]
        [Method(WindowNames.ShowMessageRequest, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
        public class ShowMessageRequestParams : IRequest<MessageActionItem>
        {
            /// <summary>
            /// The message type. See {@link MessageType}
            /// </summary>
            public MessageType Type { get; set; }

            /// <summary>
            /// The actual message
            /// </summary>
            public string Message { get; set; } = null!;

            /// <summary>
            /// The message action items to present.
            /// </summary>
            [Optional]
            public Container<MessageActionItem>? Actions { get; set; }
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class MessageActionItem
        {
            /// <summary>
            /// A short title like 'Retry', 'Open Log' etc.
            /// </summary>
            public string Title { get; set; } = null!;

            /// <summary>
            /// Extension data that may contain additional properties based on <see cref="ShowMessageRequestClientCapabilities"/>
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> ExtensionData { get; set; } = new Dictionary<string, JToken>();

            private string DebuggerDisplay => Title;

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Capabilities specific to the showMessage request
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.Window), nameof(WindowClientCapabilities.ShowMessage))]
        public class ShowMessageRequestClientCapabilities
        {
            /// <summary>
            /// Capabilities specific to the `MessageActionItem` type.
            /// </summary>
            [Optional]
            public ShowMessageRequestMessageActionItemClientCapabilities? MessageActionItem { get; set; }
        }

        [Obsolete(Constants.Proposal)]
        public class ShowMessageRequestMessageActionItemClientCapabilities
        {
            /// <summary>
            /// Whether the client supports additional attribues which
            /// are preserved and send back to the server in the
            /// request's response.
            /// </summary>
            [Optional]
            public bool AdditionalPropertiesSupport { get; set; }
        }
    }

    namespace Window
    {

    }
}
