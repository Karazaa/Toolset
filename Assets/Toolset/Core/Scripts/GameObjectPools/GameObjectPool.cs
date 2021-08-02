using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Game Object Pool for a specific MonoBehaviour. Allows for taking and returning
/// prefab instances to reduce total number of instantiation and destruction operations.
/// Useful for game objects that are frequently instantiated/destroyed without very many
/// instances of the object being active at any given point in time (ie. projectiles). 
/// </summary>
/// <typeparam name="TGameObjectPoolable">MonoBehaviour this GameObjectPool is servicing.</typeparam>
public class GameObjectPool<TGameObjectPoolable> : Singleton<GameObjectPool<TGameObjectPoolable>> where TGameObjectPoolable : MonoBehaviour
{
    private TGameObjectPoolable m_schemaInstance;
    private Queue<TGameObjectPoolable> m_inactiveGamePoolables = new Queue<TGameObjectPoolable>();

    /// <summary>
    /// Sets the prefab schema used to instantiate future instances of the MonoBehaviour.
    /// This needs to be set before being able to instantiate GameObjects.
    /// </summary>
    /// <param name="instance">An instance of the prefab used as a schema for instantiating future instances.</param>
    public void SetSchema(TGameObjectPoolable instance)
    {
        m_schemaInstance = instance;
    }

    /// <summary>
    /// Check if the Prefab schema is set.
    /// </summary>
    /// <returns>Whether or not the schema is set.</returns>
    public bool IsSchemaSet()
    {
        return m_schemaInstance != null;
    }

    /// <summary>
    /// Takes an instance of the GameObjectPool type by either instantiating it or
    /// reusing an instance that was returned to the inactive set.
    /// </summary>
    /// <returns>An instance of the GameObjectPool type.</returns>
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

    /// <summary>
    /// Returns an instance to the GameObjectPool so that it may be reused by Take operations later.
    /// </summary>
    /// <param name="instance">The instance being returned.</param>
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
}
