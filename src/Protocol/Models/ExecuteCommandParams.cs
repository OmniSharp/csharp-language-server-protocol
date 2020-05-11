using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.ExecuteCommand)]
    public class ExecuteCommandParams : IRequest, IWorkDoneProgressParams
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public JsonElement Arguments { get; set; }

        /// <inheritdoc />
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
