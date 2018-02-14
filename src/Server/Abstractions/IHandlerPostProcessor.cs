namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IHandlerPostProcessor
    {
        /// <summary>
        /// Does post processing for a request of descriptor type
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        object Process(ILspHandlerDescriptor descriptor, object parameters, object response);
    }
}
