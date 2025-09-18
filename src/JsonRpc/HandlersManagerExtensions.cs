namespace OmniSharp.Extensions.JsonRpc
{
    public static class HandlersManagerExtensions
    {
        /// <summary>
        /// Gets all the unique handlers currently registered with the manager
        /// </summary>
        /// <param name="handlersManager"></param>
        /// <returns></returns>
        public static IEnumerable<IJsonRpcHandler> GetHandlers(this IHandlersManager handlersManager)
        {
            return handlersManager.Descriptors.Select(z => z.Handler).Distinct();
        }
    }
}
