using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  The show message request is sent from a server to a client to ask the client to display a particular message in the user interface. In addition to the show message notification the request allows to pass actions and to wait for an answer from the client.
    /// </summary>
[Method(WindowNames.ShowMessageRequest)]
    public class ShowMessageRequestParams : IRequest<MessageActionItem>
    {
        /// <summary>
        ///  The message type. See {@link MessageType}
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        ///  The actual message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///  The message action items to present.
        /// </summary>
        [Optional]
        public Container<MessageActionItem> Actions { get; set; }
    }
}
