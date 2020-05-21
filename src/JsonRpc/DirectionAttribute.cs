using System;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    /// An optional enum that can be used to give hints on the direction of a given method.
    /// </summary>
    public enum Direction
    {
        Unspecified = 0b0000,
        ServerToClient = 0b0001,
        ClientToServer = 0b0010,
        Bidirectional = 0b0011
    }
}
