using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistrationManager
    {
        IObservable<IEnumerable<Registration>> Registrations { get; }
        IEnumerable<Registration> CurrentRegistrations { get; }
        IEnumerable<Registration> GetRegistrationsForMethod(string method);
        IEnumerable<Registration> GetRegistrationsMatchingSelector(TextDocumentSelector textDocumentSelector);
        void RegisterCapabilities(ServerCapabilities serverCapabilities);
    }
}
