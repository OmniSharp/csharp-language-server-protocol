namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ResponseBase
    {
        public ResponseBase(object id)
        {
            Id = id;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }
    }
}