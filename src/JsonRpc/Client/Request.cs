namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class Request
    {
        public object Id { get; set; }

        public string Method { get; set; }

        // JSONTODO
        // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Params { get; set; }
    }
}
