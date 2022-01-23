using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolset.Core
{
    public class Scope
    {
        private Dictionary<Type, IInjectable> m_dependencies = new Dictionary<Type, IInjectable>();

        public void Register<TType>(TType objectToRegister) where TType : IInjectable
        {
            m_dependencies.Add(typeof(TType), objectToRegister);
            objectToRegister.Inject(this);
        }

        public void Resolve<TType>(ref TType objectToResolve) where TType : IInjectable
        {
            objectToResolve = (TType) m_dependencies[typeof(TType)];
        }

        protected virtual IEnumerator SetUpScope()
        {
            yield break;
        }

        protected virtual IEnumerator TearDownScope()
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
