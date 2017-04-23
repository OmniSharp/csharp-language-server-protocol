using System;
using JsonRpc;

namespace Lsp
{
    public interface ILspHandlerInstance : IHandlerInstance
    {
        Type RegistrationType { get; }
    }
}