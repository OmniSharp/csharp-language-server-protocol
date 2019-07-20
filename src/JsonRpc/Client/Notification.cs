using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class Notification
    {
        public string Method { get; set; }

        public object Params { get; set; }
    }
}
