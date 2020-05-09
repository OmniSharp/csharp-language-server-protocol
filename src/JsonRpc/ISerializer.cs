using System;
using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface ISerializer
    {
        JsonSerializerOptions Options { get; }
        long GetNextId();
    }
}
