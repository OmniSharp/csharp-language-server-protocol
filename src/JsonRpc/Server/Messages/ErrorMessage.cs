namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    // JSONTODO
    // [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ErrorMessage : IErrorMessage
    {
        public ErrorMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }

        // [JsonConstructor]
        public ErrorMessage(int code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public int Code { get; }

        // JSONTODO
        // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; }

        public string Message { get; }

        object IErrorMessage.Data => Data;
    }
}
