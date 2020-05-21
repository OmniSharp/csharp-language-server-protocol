using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  The show message notification is sent from a server to a client to ask the client to display a particular message in the user interface.
    /// </summary>
[Method(WindowNames.ShowMessage, Direction.ServerToClient)]
    public class ShowMessageParams : IRequest
    {
        /// <summary>
        ///  The message type. See {@link MessageType}.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        ///  The actual message.
        /// </summary>
        public string Message { get; set; }
    }
}
