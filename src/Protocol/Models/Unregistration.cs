namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  General parameters to unregister a request or notification.
    /// </summary>
    public class Unregistration
    {
        /// <summary>
        ///  The id used to unregister the request or notification. Usually an id
        ///  provided during the register request.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  The method to unregister for.
        /// </summary>
        public string Method { get; set; }

        public static implicit operator Unregistration(Registration registration)
        {
            return new Unregistration() {
                Id = registration.Id,
                Method = registration.Method
            };
        }
    }
}
