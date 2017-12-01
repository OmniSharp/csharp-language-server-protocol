using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static GeneralNames;
    public static partial class GeneralNames
    {
        public const string Exit = "exit";
    }

    [Serial, Method(Exit)]
    public interface IExitHandler : INotificationHandler { }
}
