using System.ComponentModel;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IHandlerIdentity
    {
        [System.Text.Json.Serialization.JsonPropertyName(Constants.PrivateHandlerId)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        // ReSharper disable once InconsistentNaming
        string __identity { get; init; }
    }
}
