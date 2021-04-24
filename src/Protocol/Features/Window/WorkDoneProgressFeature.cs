using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Serial]
        [Method(WindowNames.WorkDoneProgressCreate, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
        public partial record WorkDoneProgressCreateParams : IRequest
        {
            /// <summary>
            /// The token to be used to report progress.
            /// </summary>
            public ProgressToken? Token { get; init; }
        }

        [Serial]
        [Method(WindowNames.WorkDoneProgressCancel, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IWindowLanguageClient), typeof(ILanguageClient))]
        public partial record WorkDoneProgressCancelParams : IRequest
        {
            /// <summary>
            /// The token to be used to report progress.
            /// </summary>
            public ProgressToken? Token { get; init; }
        }


        public interface IWorkDoneProgressParams
        {
            /// <summary>
            /// An optional token that a server can use to report work done progress.
            /// </summary>
            [Optional]
            ProgressToken? WorkDoneToken { get; init; }
        }

        public interface IWorkDoneProgressOptions
        {
            [Optional] bool WorkDoneProgress { get; set; }
        }

        public abstract record WorkDoneProgress
        {
            public WorkDoneProgress(WorkDoneProgressKind kind) => Kind = kind;

            public WorkDoneProgressKind Kind { get; }

            /// <summary>
            /// Optional, a final message indicating to for example indicate the outcome
            /// of the operation.
            /// </summary>
            [Optional]
            public string? Message { get; init; }
        }

        [StringEnum]
        public readonly partial struct WorkDoneProgressKind
        {
            public static WorkDoneProgressKind Begin { get; } = new WorkDoneProgressKind("begin");
            public static WorkDoneProgressKind End { get; } = new WorkDoneProgressKind("end");
            public static WorkDoneProgressKind Report { get; } = new WorkDoneProgressKind("report");
        }

        /// <summary>
        /// To start progress reporting a `$/progress` notification with the following payload must be sent
        /// </summary>
        public record WorkDoneProgressBegin : WorkDoneProgress
        {
            public WorkDoneProgressBegin() : base(WorkDoneProgressKind.Begin)
            {
            }

            /// <summary>
            /// Mandatory title of the progress operation. Used to briefly inform about
            /// the kind of operation being performed.
            ///
            /// Examples: "Indexing" or "Linking dependencies".
            /// </summary>
            public string Title { get; init; }

            /// <summary>
            /// Controls if a cancel button should show to allow the user to cancel the
            /// long running operation. Clients that don't support cancellation are allowed
            /// to ignore the setting.
            /// </summary>
            [Optional]
            public bool Cancellable { get; init; }

            /// <summary>
            /// Optional progress percentage to display (value 100 is considered 100%).
            /// If not provided infinite progress is assumed and clients are allowed
            /// to ignore the `percentage` value in subsequent in report notifications.
            ///
            /// The value should be steadily rising. Clients are free to ignore values
            /// that are not following this rule.
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            [Optional]
            public int? Percentage { get; init; }
        }

        /// <summary>
        /// Signaling the end of a progress reporting is done using the following payload
        /// </summary>
        public record WorkDoneProgressEnd : WorkDoneProgress
        {
            public WorkDoneProgressEnd() : base(WorkDoneProgressKind.End)
            {
            }
        }

        public record WorkDoneProgressOptions : IWorkDoneProgressOptions
        {
            [Optional] public bool WorkDoneProgress { get; set; }
        }

        /// <summary>
        /// Reporting progress is done using the following payload
        /// </summary>
        public record WorkDoneProgressReport : WorkDoneProgress
        {
            public WorkDoneProgressReport() : base(WorkDoneProgressKind.Report)
            {
            }

            /// <summary>
            /// Controls enablement state of a cancel button. This property is only valid if a cancel
            /// button got requested in the `WorkDoneProgressStart` payload.
            ///
            /// Clients that don't support cancellation or don't support control the button's
            /// enablement state are allowed to ignore the setting.
            /// </summary>
            [Optional]
            public bool Cancellable { get; set; }

            /// <summary>
            /// Optional progress percentage to display (value 100 is considered 100%).
            /// If not provided infinite progress is assumed and clients are allowed
            /// to ignore the `percentage` value in subsequent in report notifications.
            ///
            /// The value should be steadily rising. Clients are free to ignore values
            /// that are not following this rule.
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            [Optional]
            public int? Percentage { get; set; }
        }
    }

    namespace Window
    {
        public static partial class WorkDoneProgressExtensions
        {
            public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, IWorkDoneProgressParams @params) =>
                mediator.SendNotification(WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams { Token = @params.WorkDoneToken });

            public static void SendWorkDoneProgressCancel(this IWindowLanguageClient mediator, ProgressToken token) =>
                mediator.SendNotification(WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams { Token = token });
        }
    }
}
