using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class MessageActionItem
    {
        /// <summary>
        ///  A short title like 'Retry', 'Open Log' etc.
        /// </summary>
        public string Title { get; set; }
    }
}
