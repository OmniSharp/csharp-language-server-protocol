namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IAwaitableTermination
    {
        System.Threading.Tasks.Task WasShutDown { get; }
    }
}