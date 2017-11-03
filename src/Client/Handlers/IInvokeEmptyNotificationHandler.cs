using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     Represents a handler for empty notifications.
    /// </summary>
    public interface IInvokeEmptyNotificationHandler
        : IHandler
    {
        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        Task Invoke();
    }
}
