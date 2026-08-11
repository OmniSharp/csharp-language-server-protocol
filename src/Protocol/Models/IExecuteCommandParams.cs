using System.Text.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        Container<JsonElement>? Arguments { get; }
    }
}
