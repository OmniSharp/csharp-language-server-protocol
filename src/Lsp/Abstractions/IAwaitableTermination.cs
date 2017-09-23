namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    public interface IAwaitableTermination
    {
        System.Threading.Tasks.Task WasShutDown { get; }
    }
}