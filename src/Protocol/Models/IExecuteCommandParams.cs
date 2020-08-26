using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        string Command { get; set; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        JArray? Arguments { get; set; }
    }
}
