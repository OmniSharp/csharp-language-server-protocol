using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class RegistrationParams
    {
        public RegistrationContainer Registrations { get; set; }
    }
}
