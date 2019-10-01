using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class InitializedExtensions
    {
        public static void SendInitialized(this IDebugClient mediator, InitializedEvent @event)
        {
            mediator.SendNotification(EventNames.Initialized, @event);
        }
    }
}
