using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class CapabilitiesExtensions
    {
        public static void SendCapabilities(this IDebugClient mediator, CapabilitiesEvent @event)
        {
            mediator.SendNotification(EventNames.Capabilities, @event);
        }
    }
}
