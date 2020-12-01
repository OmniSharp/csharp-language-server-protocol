using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// A Stackframe contains the source location.
    /// </summary>
    public record StackFrame
    {
        /// <summary>
        /// An identifier for the stack frame. It must be unique across all threads. This id can be used to retrieve the scopes of the frame with the 'scopesRequest' or to restart the
        /// execution of a stackframe.
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// The name of the stack frame, typically a method name.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// The optional source of the frame.
        /// </summary>
        [Optional]
        public Source? Source { get; init; }

        /// <summary>
        /// The line within the file of the frame. If source is null or doesn't exist, line is 0 and must be ignored.
        /// </summary>
        public int Line { get; init; }

        /// <summary>
        /// The column within the line. If source is null or doesn't exist, column is 0 and must be ignored.
        /// </summary>
        public int Column { get; init; }

        /// <summary>
        /// An optional end line of the range covered by the stack frame.
        /// </summary>
        [Optional]
        public int? EndLine { get; init; }

        /// <summary>
        /// An optional end column of the range covered by the stack frame.
        /// </summary>
        [Optional]
        public int? EndColumn { get; init; }

        /// <summary>
        /// Optional memory reference for the current instruction pointer in this frame.
        /// </summary>
        [Optional]
        public string? InstructionPointerReference { get; init; }

        /// <summary>
        /// The module associated with this frame, if any.
        /// </summary>
        [Optional]
        public NumberString? ModuleId { get; init; }

        /// <summary>
        /// An optional hint for how to present this frame in the UI. A value of 'label' can be used to indicate that the frame is an artificial frame that is used as a visual label or
        /// separator. A value of 'subtle' can be used to change the appearance of a frame in a 'subtle' way.
        /// </summary>
        [Optional]
        public StackFramePresentationHint? PresentationHint { get; init; }
    }

    [StringEnum]
    public readonly partial struct StackFramePresentationHint
    {
        public static SourcePresentationHint Normal { get; } = new SourcePresentationHint("normal");
        public static SourcePresentationHint Label { get; } = new SourcePresentationHint("label");
        public static SourcePresentationHint Subtle { get; } = new SourcePresentationHint("subtle");
    }
}
