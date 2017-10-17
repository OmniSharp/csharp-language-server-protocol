using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer
{
    public delegate Task InitializeDelegate(InitializeParams request);
}