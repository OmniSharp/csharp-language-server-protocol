using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static GeneralNames;
    public static partial class GeneralNames
    {
        public const string CancelRequest = "$/cancelRequest";
    }

    [Parallel, Method(CancelRequest)]
    public interface ICancelRequestHandler : INotificationHandler<CancelParams> { }
}
