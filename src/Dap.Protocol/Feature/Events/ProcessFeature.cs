using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Process, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ProcessEvent : IRequest
        {
            /// <summary>
            /// The logical name of the process. This is usually the full path to process's executable file. Example: /home/example/myproj/program.js.
            /// </summary>
            public string Name { get; init; }

            /// <summary>
            /// The system process id of the debugged process. This property will be missing for non-system processes.
            /// </summary>
            [Optional]
            public long? SystemProcessId { get; init; }

            /// <summary>
            /// If true, the process is running on the same computer as the debug adapter.
            /// </summary>
            [Optional]
            public bool IsLocalProcess { get; init; }

            /// <summary>
            /// Describes how the debug engine started debugging this process.
            /// 'launch': Process was launched under the debugger.
            /// 'attach': Debugger attached to an existing process.
            /// 'attachForSuspendedLaunch': A project launcher component has launched a new process in a suspended state and then asked the debugger to attach.
            /// </summary>
            [Optional]
            public ProcessEventStartMethod? StartMethod { get; init; }

            /// <summary>
            /// The size of a pointer or address for this process, in bits. This value may be used by clients when formatting addresses for display.
            /// </summary>
            [Optional]
            public long? PointerSize { get; init; }
        }

        [StringEnum]
        public readonly partial struct ProcessEventStartMethod
        {
            public static ProcessEventStartMethod Launch { get; } = new ProcessEventStartMethod("launch");
            public static ProcessEventStartMethod Attach { get; } = new ProcessEventStartMethod("attach");
            public static ProcessEventStartMethod AttachForSuspendedLaunch { get; } = new ProcessEventStartMethod("attachForSuspendedLaunch");
        }
    }
}
