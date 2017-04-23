using JsonRpc;

namespace Lsp
{
    public interface IRegistrableNotificationHandler<TNotification, TRegistration> : INotificationHandler<TNotification>, IRegistration<TRegistration> { }
}