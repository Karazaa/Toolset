using System;

namespace Toolset.Core
{
    public interface IInjectable : IDisposable
    {
        void Inject(Scope scope);
    }
}
