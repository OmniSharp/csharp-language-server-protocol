using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public delegate Task InitializeDelegate(ILanguageServer server, InitializeParams request);
    public delegate Task StartedDelegate(InitializeResult result);
}
