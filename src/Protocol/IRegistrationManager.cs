using DynamicData;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistrationManager
    {
        IObservableList<Registration> Registrations { get; }
        IObservableList<Registration> GetRegistrationsForMethod(string method);
        IObservableList<Registration> GetRegistrationsMatchingSelector(DocumentSelector documentSelector);
        void RegisterCapabilities(ServerCapabilities serverCapabilities);
    }
}
