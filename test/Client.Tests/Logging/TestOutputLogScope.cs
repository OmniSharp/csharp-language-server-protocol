using System;
using System.Threading;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests.Logging
{
    /// <summary>
    ///     Log scope for <see cref="TestOutputLogger"/>.
    /// </summary>
    internal class TestOutputLogScope
    {
        /// <summary>
        ///     Storage for the current <see cref="TestOutputLogScope"/>.
        /// </summary>
        static readonly AsyncLocal<TestOutputLogScope> _currentScope = new AsyncLocal<TestOutputLogScope>();

        /// <summary>
        ///     The current <see cref="TestOutputLogScope"/> (if any).
        /// </summary>
        public static TestOutputLogScope Current
        {
            get => _currentScope.Value;
            set => _currentScope.Value = value;
        }

        /// <summary>
        ///     The scope name.
        /// </summary>
        readonly string _name;

        /// <summary>
        ///     State associated with the scope.
        /// </summary>
        readonly object _state;

        /// <summary>
        ///     Create a new <see cref="TestOutputLogScope"/>.
        /// </summary>
        /// <param name="name">
        ///     The scope name.
        /// </param>
        /// <param name="state">
        ///     State associated with the scope.
        /// </param>
        public TestOutputLogScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        /// <summary>
        ///     The scope's parent scope (if any).
        /// </summary>
        public TestOutputLogScope Parent { get; private set; }

        /// <summary>
        ///     Create a new <see cref="TestOutputLogScope"/> and make it the current <see cref="TestOutputLogScope"/>.
        /// </summary>
        /// <param name="name">
        ///     The scope name.
        /// </param>
        /// <param name="state">
        ///     State associated with the scope.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the scope.
        /// </returns>
        public static IDisposable Push(string name, object state)
        {
            TestOutputLogScope parent = Current;
            Current = new TestOutputLogScope(name, state) {
                Parent = parent
            };

            return new ScopeDisposal();
        }

        /// <summary>
        ///     Get a string representation of the scope.
        /// </summary>
        /// <returns>
        ///     The scope's string representation.
        /// </returns>
        public override string ToString() => _state?.ToString();

        /// <summary>
        ///     Wrapper for disposal of log scope.
        /// </summary>
        class ScopeDisposal
            : IDisposable
        {
            /// <summary>
            ///     Has the scope been disposed?
            /// </summary>
            bool _disposed;

            /// <summary>
            ///     Revert to the previous scope (if any).
            /// </summary>
            public void Dispose()
            {
                if (_disposed)
                    return;

                Current = Current?.Parent;
                _disposed = true;
            }
        }
    }
}
