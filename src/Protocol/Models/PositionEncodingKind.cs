using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

/// <summary>
/// A set of predefined position encoding kinds.
///
/// @since 3.17.0
/// </summary>
[StringEnum]
public readonly partial struct PositionEncodingKind
{
    /// <summary>
    /// Character offsets count UTF-8 code units.
    /// </summary>
    public static PositionEncodingKind UTF8 = new("utf-8");

    /// <summary>
    /// Character offsets count UTF-16 code units.
    ///
    /// This is the default and must always be supported
    /// by servers
    /// </summary>
    public static PositionEncodingKind UTF16 = new("utf-16");

    /// <summary>
    /// Character offsets count UTF-32 code units.
    ///
    /// Implementation note: these are the same as Unicode code points,
    /// so this `PositionEncodingKind` may also be used for an
    /// encoding-agnostic representation of character offsets.
    /// </summary>
    public static PositionEncodingKind UTF32 = new("utf-32");
}
