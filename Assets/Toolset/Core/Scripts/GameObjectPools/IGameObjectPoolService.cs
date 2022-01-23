using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// Interface that specifies necessary API for a GameObjectPool.
    /// </summary>
    /// <typeparam name="TGameObjectPoolable">MonoBehaviour this GameObjectPool is servicing.</typeparam>
    public interface IGameObjectPoolService<TGameObjectPoolable> : IInjectable where TGameObjectPoolable : MonoBehaviour
    {
        /// <summary>
        /// Sets the prefab schema used to instantiate future instances of the MonoBehaviour.
        /// This needs to be set before being able to instantiate GameObjects.
        /// </summary>
        /// <param name="instance">An instance of the prefab used as a schema for instantiating future instances.</param>
        public void SetSchema(TGameObjectPoolable instance);

        /// <summary>
        /// Check if the Prefab schema is set.
        /// </summary>
        /// <returns>Whether or not the schema is set.</returns>
        public bool IsSchemaSet();

        /// <summary>
        /// Takes an instance of the GameObjectPool type by either instantiating it or
        /// reusing an instance that was returned to the inactive set.
        /// </summary>
        /// <returns>An instance of the GameObjectPool type.</returns>
        public TGameObjectPoolable Take();

        /// <summary>
        /// Returns an instance to the GameObjectPool so that it may be reused by Take operations later.
        /// </summary>
        /// <param name="instance">The instance being returned.</param>
        public void Return(TGameObjectPoolable instance);
    }
}
