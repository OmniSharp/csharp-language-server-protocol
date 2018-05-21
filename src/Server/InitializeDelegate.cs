using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public delegate Task InitializeDelegate(InitializeParams request);
}
