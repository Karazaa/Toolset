using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolset.Core
{
    public static class ScopeUtils
    {
        public static Scope CurrentScope { get; private set; }

        public static IEnumerator CreateScope<TScope>() 
            where TScope : Scope, new()
        {
            Scope scope = new TScope();
            CurrentScope = scope;

            yield return scope.SetUpScope();
        }

        public static IEnumerator DestroyScope()
        {
            Scope parentScope = CurrentScope.ParentScope;

            yield return CurrentScope.TearDownScope();

            CurrentScope = parentScope;
        }
    }

    public abstract class Scope
    {
        public Scope ParentScope { get; private set; }
        private Dictionary<Type, IInjectable> m_dependencies = new Dictionary<Type, IInjectable>();

        public Scope ()
        {
            ParentScope = ScopeUtils.CurrentScope;
        }

        protected void Register<TType>(TType objectToRegister) where TType : IInjectable
        {
            m_dependencies.Add(typeof(TType), objectToRegister);
            objectToRegister.Inject(this);
        }

        protected void Resolve<TType>(ref TType objectToResolve) where TType : IInjectable
        {
            objectToResolve = (TType) m_dependencies[typeof(TType)];
        }

        public abstract IEnumerator SetUpScope();

        public virtual IEnumerator TearDownScope()
        {
            foreach (KeyValuePair<Type, IInjectable> pair in m_dependencies)
            {
                pair.Value.Dispose();
            }

            m_dependencies.Clear();

            yield break;
        }
    }
}
