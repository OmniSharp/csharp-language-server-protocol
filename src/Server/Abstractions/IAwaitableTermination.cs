using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IAwaitableTermination
    {
        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }
}
