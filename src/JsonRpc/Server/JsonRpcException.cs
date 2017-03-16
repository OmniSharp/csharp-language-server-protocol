using System;
using System.Runtime.Serialization;

namespace JsonRPC.Server
{
    public class JsonRpcException : Exception
    {
        public JsonRpcException()
        {
        }

        public JsonRpcException(string message) : base(message)
        {
        }

        public JsonRpcException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //protected JsonRpcException(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //}
    }
}