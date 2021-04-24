namespace OmniSharp.Extensions.JsonRpc.Client
{
    public record OutgoingNotification
    {
        public string Method { get; set; } = null!;

        public object? Params { get; set; }
    }
}
