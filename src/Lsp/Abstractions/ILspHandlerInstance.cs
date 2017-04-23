using System;
using JsonRpc;
using Lsp.Models;

namespace Lsp
{
    public interface ILspHandlerInstance : IHandlerInstance
    {
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        Registration Registration { get; }

        bool HasCapability { get; }
        Type CapabilityType { get; }
        void SetCapability(object instance);
        bool IsDynamicCapability { get; }
        bool AllowsDynamicRegistration { get; }
    }
}