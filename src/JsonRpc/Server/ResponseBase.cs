namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ResponseBase
    {
        public ResponseBase(object id)
        {
            Id = id;
        }

        public object Id { get; set; }
    }
}
