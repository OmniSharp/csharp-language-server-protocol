using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class StoppedExtensions
    {
        public static void SendStopped(this IDebugClient mediator, StoppedEvent @event)
        {
            mediator.SendNotification(EventNames.Stopped, @event);
        }
    }
}
