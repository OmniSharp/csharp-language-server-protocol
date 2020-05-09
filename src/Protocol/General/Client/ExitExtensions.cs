

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class ExitExtensions
    {
        public static void Exit(this ILanguageClient mediator)
        {
            mediator.SendNotification(GeneralNames.Exit);
        }
    }
}
