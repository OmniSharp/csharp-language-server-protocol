using System;

namespace OmniSharp.Extensions.JsonRpc
{
    public class InputProcessingException : Exception
    {
        public InputProcessingException(string message, Exception innerException) : base($"There was an error processing input the contents of the buffer were '{message}'", innerException)
        {

        }
    }
}