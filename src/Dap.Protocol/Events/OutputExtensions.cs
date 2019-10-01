using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class OutputExtensions
    {
        public static void SendOutput(this IDebugClient mediator, OutputEvent @event)
        {
            mediator.SendNotification(EventNames.Output, @event);
        }
    }
}
