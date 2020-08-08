using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ILanguageProtocolSettings
    {
        InitializeParams ClientSettings { get;   }

        InitializeResult ServerSettings { get;   }
    }
}