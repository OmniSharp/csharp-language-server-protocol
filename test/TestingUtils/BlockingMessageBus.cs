// See https://github.com/JoshKeegan/xRetry

using System.Collections.Concurrent;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestingUtils
{
    /// <summary>
    /// An XUnit message bus that can block messages from being passed until we want them to be.
    /// </summary>
    public class BlockingMessageBus : IMessageBus
    {
        private readonly IMessageBus _underlyingMessageBus;
        private ConcurrentQueue<IMessageSinkMessage> _messageQueue = new ConcurrentQueue<IMessageSinkMessage>();

        public BlockingMessageBus(IMessageBus underlyingMessageBus)
        {
            _underlyingMessageBus = underlyingMessageBus;
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            _messageQueue.Enqueue(message);

            // Returns if execution should continue. Since we are intercepting the message, we
            //  have no way of checking this so always continue...
            return true;
        }

        public void Clear()
        {
            _messageQueue = new ConcurrentQueue<IMessageSinkMessage>();
        }

        /// <summary>
        /// Write the cached messages to the underlying message bus
        /// </summary>
        public void Flush()
        {
            while (_messageQueue.TryDequeue(out IMessageSinkMessage message))
            {
                _underlyingMessageBus.QueueMessage(message);
            }
        }

        public void Dispose()
        {
            // Do not dispose of the underlying message bus - it is an externally owned resource
        }
    }
}
