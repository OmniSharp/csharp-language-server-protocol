namespace JsonRpc.Server.Messages
{
    public class RequestCancelled : Error
    {
        internal RequestCancelled() : base(null, new ErrorMessage(-32800, "Request Cancelled")) { }
    }
}