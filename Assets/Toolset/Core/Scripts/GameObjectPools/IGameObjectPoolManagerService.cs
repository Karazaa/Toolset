using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// Interface that specifies necessary API for GameObjectPoolManager
    /// </summary>
    public interface IGameObjectPoolManagerService : IInjectable
    {
        /// <summary>
        /// Transform that all active GameObjectPool instantiated objects are a child of.
        /// </summary>
        public Transform ActiveRoot { get; }

        /// <summary>
        /// Transform that all inactive GameObjectPool instantiated objects are a child of.
        /// </summary>
        public Transform InactiveRoot { get; }

        /// <summary>
        /// Creates an instance of a game object based on the passed schema prefab. Used by
        /// various GameObjectPools to create new GameObjects if needed.
        /// </summary>
        /// <typeparam name="TGameObjectPoolable">The type of poolable to be created.</typeparam>
        /// <param name="schema">An instance of the poolable to use as a prefab schema for instantiation.</param>
        /// <returns>The newly created instance of the poolable.</returns>
        public TGameObjectPoolable CreateInstance<TGameObjectPoolable>(TGameObjectPoolable schema) where TGameObjectPoolable : MonoBehaviour;
    }
}
