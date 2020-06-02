namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class OutgoingNotification
    {
        public string Method { get; set; }

        public object Params { get; set; }
    }
}
