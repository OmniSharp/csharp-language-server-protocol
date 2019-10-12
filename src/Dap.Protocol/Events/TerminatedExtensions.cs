using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class TerminatedExtensions
    {
        public static void SendTerminated(this IDebugClient mediator, TerminatedEvent @event)
        {
            mediator.SendNotification(EventNames.Terminated, @event);
        }
    }
}
