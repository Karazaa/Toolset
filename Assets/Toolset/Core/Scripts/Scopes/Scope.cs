using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolset.Core
{
    /// <summary>
    /// Utility class that facilitates scope creation and deletion.
    /// </summary>
    public static class ScopeUtils
    {
        public static Scope CurrentScope { get; private set; }

        /// <summary>
        /// Creates a scope, yields on its SetUpScope method, and sets it as the CurrentScope.
        /// </summary>
        /// <typeparam name="TScope">The type of scope to create</typeparam>
        /// <returns>The CreateScope routine.</returns>
        public static IEnumerator CreateScope<TScope>() 
            where TScope : Scope, new()
        {
            Scope scope = new TScope();
            CurrentScope = scope;

            yield return scope.SetUpScope();
        }

        /// <summary>
        /// Destroys the CurrentScope, yields on its TearDownScope routine, and sets its parent as the new CurrentScope.
        /// </summary>
        /// <returns>The DestroyScope routine.</returns>
        public static IEnumerator DestroyScope()
        {
            Scope parentScope = CurrentScope.ParentScope;

            yield return CurrentScope.TearDownScope();

            CurrentScope = parentScope;
        }

        /// <summary>
        /// Calls DestroyScope on all Scopes in the CurrentScope chain.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator DestroyAllScopes()
        {
            while (CurrentScope != null)
            {
                yield return DestroyScope();
            }
        }
    }

    /// <summary>
    /// Abstract class that defines a set of injectable dependencies that can be passed to IInjectables. Has a reference to its parent Scope.
    /// </summary>
    public abstract class Scope
    {
        /// <summary>
        /// The scope that encompases this scope.
        /// </summary>
        public Scope ParentScope { get; private set; }
        private Dictionary<Type, IInjectable> m_dependencies = new Dictionary<Type, IInjectable>();

        public Scope ()
        {
            ParentScope = ScopeUtils.CurrentScope;
        }

        protected void Register<TType>(TType objectToRegister) where TType : IInjectable
        {
            if (m_dependencies.ContainsKey(typeof(TType)))
                throw new InvalidOperationException("[Toolset.Scope] Attempted to register service of type {0}, which is already registered to scope {1}!".StringBuilderFormat(typeof(TType), this));

            m_dependencies.Add(typeof(TType), objectToRegister);
            objectToRegister.Inject(this);
        }

        protected void Resolve<TType>(ref TType objectToResolve) where TType : IInjectable
        {
            if (m_dependencies.ContainsKey(typeof(TType)))
                objectToResolve = (TType)m_dependencies[typeof(TType)];
            else
                ParentScope?.Resolve(ref objectToResolve);

            if (objectToResolve == null)
                throw new InvalidOperationException("[Toolset.Scope] Attempted to resolve object of type {0}, but no instances are registered to any scopes!".StringBuilderFormat(typeof(TType)));
        }

        /// <summary>
        /// Stands up the scope which entails resolving dependencies and registering new services to the scope.
        /// </summary>
        /// <returns>The SetUpScope routine.</returns>
        public abstract IEnumerator SetUpScope();

        /// <summary>
        /// Tears down the scope which entails disposing all registered services.
        /// </summary>
        /// <returns>The TearDownScope routine.</returns>
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
