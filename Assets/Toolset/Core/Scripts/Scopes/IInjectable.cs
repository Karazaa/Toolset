using System;

namespace Toolset.Core
{
    /// <summary>
    /// Interface that defines the Injectable API
    /// </summary>
    public interface IInjectable : IDisposable
    {
        /// <summary>
        /// Passes a scope to the Injectable in order to allow it to resolve dependencies.
        /// </summary>
        /// <param name="scope">The scope that will provide dependency resolution.</param>
        void Inject(Scope scope);
    }
}
