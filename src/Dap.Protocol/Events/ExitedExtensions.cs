using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ExitedExtensions
    {
        public static void SendExited(this IDebugClient mediator, ExitedEvent @event)
        {
            mediator.SendNotification(EventNames.Exited, @event);
        }
    }
}
