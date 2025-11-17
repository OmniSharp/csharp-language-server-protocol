namespace OmniSharp.Extensions.JsonRpc
{
    public sealed class RequestInvokerOptions
    {
        public RequestInvokerOptions(
            TimeSpan requestTimeout,
            bool supportContentModified,
            int concurrency)
        {
            RequestTimeout = requestTimeout;
            SupportContentModified = supportContentModified;
            Concurrency = concurrency > 1 ? concurrency : null;
        }

        public TimeSpan RequestTimeout { get; }

        public bool SupportContentModified { get; }

        public int? Concurrency { get; }
    }
}
