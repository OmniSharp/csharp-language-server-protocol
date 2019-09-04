using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ProcessExtensions
    {
        public static void SendProcess(this IDebugClient mediator, ProcessEvent @event)
        {
            mediator.SendNotification(EventNames.Process, @event);
        }
    }
}
