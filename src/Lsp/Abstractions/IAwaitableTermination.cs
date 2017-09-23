namespace OmniSharp.Extensions.LanguageServerProtocol.Abstractions
{
    public interface IAwaitableTermination
    {
        System.Threading.Tasks.Task WasShutDown { get; }
    }
}