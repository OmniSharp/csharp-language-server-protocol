using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// VariablePresentationHint
    /// Optional properties of a variable that can be used to determine how to render the variable in the UI.
    /// </summary>
    public record VariablePresentationHint
    {
        /// <summary>
        /// The kind of variable. Before introducing additional values, try to use the listed values.
        /// Values:
        /// 'property': Indicates that the object is a property.
        /// 'method': Indicates that the object is a method.
        /// 'class': Indicates that the object is a class.
        /// 'data': Indicates that the object is data.
        /// 'event': Indicates that the object is an event.
        /// 'baseClass': Indicates that the object is a base class.
        /// 'innerClass': Indicates that the object is an inner class.
        /// 'interface': Indicates that the object is an interface.
        /// 'mostDerivedClass': Indicates that the object is the most derived class.
        /// 'virtual': Indicates that the object is virtual, that means it is a synthetic object introduced by the adapter for rendering purposes, e.g. an index range for large arrays.
        /// 'dataBreakpoint': Indicates that a data breakpoint is registered for the object.
        /// etc.
        /// </summary>
        [Optional]
        public VariablePresentationHintKind? Kind { get; init; }

        /// <summary>
        /// Set of attributes represented as an array of strings. Before introducing additional values, try to use the listed values.
        /// Values:
        /// 'static': Indicates that the object is static.
        /// 'constant': Indicates that the object is a constant.
        /// 'readOnly': Indicates that the object is read only.
        /// 'rawString': Indicates that the object is a raw string.
        /// 'hasObjectId': Indicates that the object can have an Object ID created for it.
        /// 'canHaveObjectId': Indicates that the object has an Object ID associated with it.
        /// 'hasSideEffects': Indicates that the evaluation had side effects.
        /// etc.
        /// </summary>
        [Optional]
        public Container<VariableAttributes>? Attributes { get; init; }

        /// <summary>
        /// Visibility of variable. Before introducing additional values, try to use the listed values.
        /// Values: 'public', 'private', 'protected', 'internal', 'final', etc.
        /// </summary>
        [Optional]
        public VariableVisibility? Visibility { get; init; }
    }

    [StringEnum]
    public readonly partial struct VariablePresentationHintKind
    {
        public static VariablePresentationHintKind Property { get; } = new("property");
        public static VariablePresentationHintKind Method { get; } = new("method");
        public static VariablePresentationHintKind Class { get; } = new("class");
        public static VariablePresentationHintKind Data { get; } = new("data");
        public static VariablePresentationHintKind Event { get; } = new("event");
        public static VariablePresentationHintKind BaseClass { get; } = new("baseClass");
        public static VariablePresentationHintKind InnerClass { get; } = new("innerClass");
        public static VariablePresentationHintKind Interface { get; } = new("interface");
        public static VariablePresentationHintKind MostDerivedClass { get; } = new("mostDerivedClass");
        public static VariablePresentationHintKind Virtual { get; } = new("virtual");
        public static VariablePresentationHintKind DataBreakpoint { get; } = new("dataBreakpoint");
    }

    [StringEnum]
    public readonly partial struct VariableAttributes
    {
        public static VariableAttributes Static { get; } = new("static");
        public static VariableAttributes Constant { get; } = new("constant");
        public static VariableAttributes ReadOnly { get; } = new("readOnly");
        public static VariableAttributes RawString { get; } = new("rawString");
        public static VariableAttributes HasObjectId { get; } = new("hasObjectId");
        public static VariableAttributes CanHaveObjectId { get; } = new("canHaveObjectId");
        public static VariableAttributes HasSideEffects { get; } = new("hasSideEffects");
    }

    [StringEnum]
    public readonly partial struct VariableVisibility
    {
        public static VariableVisibility Public { get; } = new("public");
        public static VariableVisibility Private { get; } = new("private");
        public static VariableVisibility Protected { get; } = new("protected");
        public static VariableVisibility Internal { get; } = new("internal");
        public static VariableVisibility Final { get; } = new("final");
    }
}
