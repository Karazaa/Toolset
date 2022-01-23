using UnityEngine;
using System;
using System.Collections.Generic;

namespace Toolset.Core
{
    /// <summary>
    /// Game Object Pool for a specific MonoBehaviour. Allows for taking and returning
    /// prefab instances to reduce total number of instantiation and destruction operations.
    /// Useful for game objects that are frequently instantiated/destroyed without very many
    /// instances of the object being active at any given point in time (ie. projectiles). 
    /// </summary>
    /// <typeparam name="TGameObjectPoolable">MonoBehaviour this GameObjectPool is servicing.</typeparam>
    public class GameObjectPool<TGameObjectPoolable> : Singleton<GameObjectPool<TGameObjectPoolable>>, IGameObjectPoolService<TGameObjectPoolable> where TGameObjectPoolable : MonoBehaviour
    {
        private TGameObjectPoolable m_schemaInstance;
        private Queue<TGameObjectPoolable> m_inactiveGamePoolables = new Queue<TGameObjectPoolable>();

        public void SetSchema(TGameObjectPoolable instance)
        {
            m_schemaInstance = instance;
        }

        public bool IsSchemaSet()
        {
            return m_schemaInstance != null;
        }

        public virtual TGameObjectPoolable Take()
        {
            HasSchemaOrThrow();

            TGameObjectPoolable instance;
            if (m_inactiveGamePoolables.Count == 0)
                instance = GameObjectPoolManager.I.CreateInstance(m_schemaInstance);
            else
                instance = m_inactiveGamePoolables.Dequeue();

            instance.transform.parent = GameObjectPoolManager.I.ActiveRoot;
            instance.gameObject.SetActive(true);
            return instance;
        }

        public virtual void Return(TGameObjectPoolable instance)
        {
            HasSchemaOrThrow();

            instance.gameObject.SetActive(false);
            instance.transform.parent = GameObjectPoolManager.I.InactiveRoot;
            m_inactiveGamePoolables.Enqueue(instance);
        }

        private void HasSchemaOrThrow()
        {
            if (!IsSchemaSet())
                throw new InvalidOperationException("[Toolset.GameObjectPool] Pool of type {0} does not have a prefab schema set yet! A schema needs to be set before Take or Return operations can occur."
                                                        .StringBuilderFormat(typeof(TGameObjectPoolable).Name));
        }

        public void Inject(Scope scope)
        {
            // Resolve dependencies if there are any.
        }

        public void Dispose()
        {
            m_schemaInstance = null;
            m_inactiveGamePoolables.Clear();
        }
    }
}