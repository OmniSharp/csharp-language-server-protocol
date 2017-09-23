using System;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class JsonRpcInvalidRequestException : JsonRpcException
    {
        public JsonRpcInvalidRequestException()
        {
        }

        public JsonRpcInvalidRequestException(string message) : base(message)
        {
        }

        public JsonRpcInvalidRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //protected JsonRpcInvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //}
    }
}