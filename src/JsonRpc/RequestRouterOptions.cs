using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RequestRouterOptions
    {
        public TimeSpan MaximumRequestTimeout { get; set; } = TimeSpan.FromMinutes(5);
    }
}
