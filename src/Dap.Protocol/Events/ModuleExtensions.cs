using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ModuleExtensions
    {
        public static void SendModule(this IDebugClient mediator, ModuleEvent @event)
        {
            mediator.SendNotification(EventNames.Module, @event);
        }
    }
}
