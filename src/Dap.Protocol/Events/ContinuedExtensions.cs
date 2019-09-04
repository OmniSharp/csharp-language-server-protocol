using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ContinuedExtensions
    {
        public static void SendContinued(this IDebugClient mediator, ContinuedEvent @event)
        {
            mediator.SendNotification(EventNames.Continued, @event);
        }
    }
}
