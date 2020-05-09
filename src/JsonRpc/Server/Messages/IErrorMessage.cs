namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public interface IErrorMessage
    {
        int Code { get; }

        string Message { get; }

        // JSONTODO
        // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        object Data { get; }
    }
}
