using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistrationManager
    {
        IObservable<IEnumerable<Registration>> Registrations { get; }
        IEnumerable<Registration> CurrentRegistrations { get; }
        IEnumerable<Registration> GetRegistrationsForMethod(string method);
        IEnumerable<Registration> GetRegistrationsMatchingSelector(DocumentSelector documentSelector);
        void RegisterCapabilities(ServerCapabilities serverCapabilities);
    }
}
