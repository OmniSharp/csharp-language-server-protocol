namespace Lsp
{
    public interface IAwaitableTermination
    {
        System.Threading.Tasks.Task WasShutDown { get; }
    }
}