// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <remarks>
    /// Based on https://github.com/microsoft/vs-streamjsonrpc IActivityTracingStrategy
    /// </remarks>
    public interface IActivityTracingStrategy
    {
        void ApplyOutgoing(ITraceData data);
        IDisposable? ApplyInbound(ITraceData data);
    }
    internal static class Hex
    {
        private static readonly byte[] HexBytes = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f' };
        private static readonly byte[] ReverseHexDigits = BuildReverseHexDigits();

        internal static void Encode(ReadOnlySpan<byte> src, ref Span<char> dest)
        {
            Span<byte> bytes = MemoryMarshal.Cast<char, byte>(dest);

            // Inspired by http://stackoverflow.com/questions/623104/c-byte-to-hex-string/3974535#3974535
            int lengthInNibbles = src.Length * 2;

            for (int i = 0; i < (lengthInNibbles & -2); i++)
            {
                int index0 = +i >> 1;
                var b = (byte)(src[index0] >> 4);
                bytes[(2 * i) + 1] = 0;
                bytes[2 * i++] = HexBytes[b];

                b = (byte)(src[index0] & 0x0F);
                bytes[(2 * i) + 1] = 0;
                bytes[2 * i] = HexBytes[b];
            }

            dest = dest.Slice(lengthInNibbles);
        }

        internal static void Decode(ReadOnlySpan<char> value, Span<byte> bytes)
        {
            for (int i = 0; i < value.Length; i++)
            {
                int c1 = ReverseHexDigits[value[i++] - '0'] << 4;
                int c2 = ReverseHexDigits[value[i] - '0'];

                bytes[i >> 1] = (byte)(c1 + c2);
            }
        }

        private static byte[] BuildReverseHexDigits()
        {
            var bytes = new byte['f' - '0' + 1];

            for (int i = 0; i < 10; i++)
            {
                bytes[i] = (byte)i;
            }

            for (int i = 10; i < 16; i++)
            {
                bytes[i + 'a' - '0' - 0x0a] = (byte)i;
                bytes[i + 'A' - '0' - 0x0a] = (byte)i;
            }

            return bytes;
        }
    }

    internal unsafe struct TraceParent
    {
        internal const int VersionByteCount = 1;
        internal const int ParentIdByteCount = 8;
        internal const int TraceIdByteCount = 16;
        internal const int FlagsByteCount = 1;

        internal byte Version;

        internal fixed byte TraceId[TraceIdByteCount];

        internal fixed byte ParentId[ParentIdByteCount];

        internal TraceFlags Flags;

        internal TraceParent(string? traceparent)
        {
            if (traceparent is null)
            {
                this.Version = 0;
                this.Flags = TraceFlags.None;
                return;
            }

            ReadOnlySpan<char> traceparentChars = traceparent.AsSpan();

            // Decode version
            ReadOnlySpan<char> slice = Consume(ref traceparentChars, VersionByteCount * 2);
            fixed (byte* pVersion = &this.Version)
            {
                Hex.Decode(slice, new Span<byte>(pVersion, 1));
            }

            ConsumeHyphen(ref traceparentChars);

            // Decode traceid
            slice = Consume(ref traceparentChars, TraceIdByteCount * 2);
            fixed (byte* pTraceId = this.TraceId)
            {
                Hex.Decode(slice, new Span<byte>(pTraceId, TraceIdByteCount));
            }

            ConsumeHyphen(ref traceparentChars);

            // Decode parentid
            slice = Consume(ref traceparentChars, ParentIdByteCount * 2);
            fixed (byte* pParentId = this.ParentId)
            {
                Hex.Decode(slice, new Span<byte>(pParentId, ParentIdByteCount));
            }

            ConsumeHyphen(ref traceparentChars);

            // Decode flags
            slice = Consume(ref traceparentChars, FlagsByteCount * 2);
            fixed (TraceFlags* pFlags = &this.Flags)
            {
                Hex.Decode(slice, new Span<byte>(pFlags, 1));
            }

            static void ConsumeHyphen(ref ReadOnlySpan<char> value)
            {
                if (value[0] != '-')
                {
                    Requires.Fail("Invalid format.");
                }

                value = value.Slice(1);
            }

            ReadOnlySpan<char> Consume(ref ReadOnlySpan<char> buffer, int length)
            {
                ReadOnlySpan<char> result = buffer.Slice(0, length);
                buffer = buffer.Slice(length);
                return result;
            }
        }

        [Flags]
        internal enum TraceFlags : byte
        {
            /// <summary>
            /// No flags.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// The parent is tracing their action.
            /// </summary>
            Sampled = 0x1,
        }

        internal Guid TraceIdGuid
        {
            get
            {
                fixed (byte* pTraceId = this.TraceId)
                {
                    return CopyBufferToGuid(new ReadOnlySpan<byte>(pTraceId, TraceIdByteCount));
                }
            }
        }

        public override string ToString()
        {
            // When calculating the number of characters required, double each 'byte' we have to encode since we're using hex.
            Span<char> traceparent = stackalloc char[(VersionByteCount * 2) + 1 + (TraceIdByteCount * 2) + 1 + (ParentIdByteCount * 2) + 1 + (FlagsByteCount * 2)];
            Span<char> traceParentRemaining = traceparent;

            fixed (byte* pVersion = &this.Version)
            {
                Hex.Encode(new ReadOnlySpan<byte>(pVersion, 1), ref traceParentRemaining);
            }

            AddHyphen(ref traceParentRemaining);

            fixed (byte* pTraceId = this.TraceId)
            {
                Hex.Encode(new ReadOnlySpan<byte>(pTraceId, TraceIdByteCount), ref traceParentRemaining);
            }

            AddHyphen(ref traceParentRemaining);

            fixed (byte* pParentId = this.ParentId)
            {
                Hex.Encode(new ReadOnlySpan<byte>(pParentId, ParentIdByteCount), ref traceParentRemaining);
            }

            AddHyphen(ref traceParentRemaining);

            fixed (TraceFlags* pFlags = &this.Flags)
            {
                Hex.Encode(new ReadOnlySpan<byte>(pFlags, 1), ref traceParentRemaining);
            }

            Debug.Assert(traceParentRemaining.Length == 0, "Characters were not initialized.");

            fixed (char* pValue = traceparent)
            {
                return new string(pValue, 0, traceparent.Length);
            }

            static void AddHyphen(ref Span<char> value)
            {
                value[0] = '-';
                value = value.Slice(1);
            }
        }

        private static unsafe Guid CopyBufferToGuid(ReadOnlySpan<byte> buffer)
        {
            Debug.Assert(buffer.Length == 16, "Guid buffer length mismatch.");
            fixed (byte* pBuffer = buffer)
            {
                return *(Guid*)pBuffer;
            }
        }
    }

    public sealed class CorrelationManagerTracingStrategy : IActivityTracingStrategy
    {
        private static readonly AsyncLocal<string?> TraceStateAsyncLocal = new AsyncLocal<string?>();

        /// <summary>
        /// Gets or sets the contextual <c>tracestate</c> value.
        /// </summary>
        public static string? TraceState
        {
            get => TraceStateAsyncLocal.Value;
            set => TraceStateAsyncLocal.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Diagnostics.TraceSource"/> that will receive the activity transfer, start and stop events .
        /// </summary>
        public TraceSource? TraceSource { get; set; }

        public unsafe void ApplyOutgoing(ITraceData data)
        {
            if (Trace.CorrelationManager.ActivityId != Guid.Empty)
            {
                var traceparent = default(TraceParent);

                FillRandomBytes(new Span<byte>(traceparent.ParentId, TraceParent.ParentIdByteCount));
                CopyGuidToBuffer(Trace.CorrelationManager.ActivityId, new Span<byte>(traceparent.TraceId, TraceParent.TraceIdByteCount));

                if (this.TraceSource is object && (this.TraceSource.Switch.Level & SourceLevels.ActivityTracing) == SourceLevels.ActivityTracing && this.TraceSource.Listeners.Count > 0)
                {
                    traceparent.Flags |= TraceParent.TraceFlags.Sampled;
                }

                data.TraceParent = traceparent.ToString();
                data.TraceState = TraceState;
            }
        }

        /// <inheritdoc/>
        public unsafe IDisposable? ApplyInbound(ITraceData request)
        {
            var traceparent = new TraceParent(request.TraceParent);
            Guid childActivityId = Guid.NewGuid();
            string? activityName = request is IMethodWithParams p ? p.Method : null;

            return new ActivityState(request, this.TraceSource, activityName, traceparent.TraceIdGuid, childActivityId);
        }

        private static void FillRandomBytes(Span<byte> buffer) => CopyGuidToBuffer(Guid.NewGuid(), buffer);

        private unsafe static void CopyGuidToBuffer(Guid guid, Span<byte> buffer)
        {
            ReadOnlySpan<byte> guidBytes = new ReadOnlySpan<byte>(&guid, sizeof(Guid));
            guidBytes.Slice(0, buffer.Length).CopyTo(buffer);
        }

        private class ActivityState : IDisposable
        {
            private readonly TraceSource? traceSource;
            private readonly Guid originalActivityId;
            private readonly string? originalTraceState;
            private readonly string? activityName;
            private readonly Guid parentTraceId;

            internal ActivityState(ITraceData request, TraceSource? traceSource, string? activityName, Guid parentTraceId, Guid childTraceId)
            {
                this.originalActivityId = Trace.CorrelationManager.ActivityId;
                this.originalTraceState = TraceState;
                this.activityName = activityName;
                this.parentTraceId = parentTraceId;

                if (traceSource is object && parentTraceId != Guid.Empty)
                {
                    // We set ActivityId to a short-lived value here for the sake of the TraceTransfer call that comes next.
                    // TraceTransfer goes from the current activity to the one passed as an argument.
                    // Without a traceSource object, there's no transfer and thus no need to set this temporary ActivityId.
                    Trace.CorrelationManager.ActivityId = parentTraceId;
                    traceSource.TraceTransfer(0, nameof(TraceEventType.Transfer), childTraceId);
                }

                Trace.CorrelationManager.ActivityId = childTraceId;
                TraceState = request.TraceState;

                traceSource?.TraceEvent(TraceEventType.Start, 0, this.activityName);

                this.traceSource = traceSource;
            }

            public void Dispose()
            {
                this.traceSource?.TraceEvent(TraceEventType.Stop, 0, this.activityName);

                if (this.parentTraceId != Guid.Empty)
                {
                    this.traceSource?.TraceTransfer(0, nameof(TraceEventType.Transfer), this.parentTraceId);
                }

                Trace.CorrelationManager.ActivityId = this.originalActivityId;
                TraceState = this.originalTraceState;
            }
        }
    }

    public interface ITraceData
    {


        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>traceparent</c> value.
        /// </summary>
         string? TraceParent { get; set; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>tracestate</c> value.
        /// </summary>
         string? TraceState { get; set; }
    }
}
