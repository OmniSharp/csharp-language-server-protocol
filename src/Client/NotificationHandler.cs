using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     A handler for empty notifications.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task"/> representing the operation.
    /// </returns>
    public delegate void NotificationHandler();
}