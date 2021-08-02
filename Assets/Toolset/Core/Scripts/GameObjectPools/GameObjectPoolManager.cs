using UnityEngine;

/// <summary>
/// MonoBehavior Singleton used to facilitate the instantation/organization of GameObjects from various GameObjectPools. This class is
/// needed as a component of the GameObjectPool implementation pattern since MonoBehaviors cannot be generic, but the pools are generic.
/// </summary>
public class GameObjectPoolManager : MonoBehaviorSingleton<GameObjectPoolManager>
{
    /// <summary>
    /// Transform that all active GameObjectPool instantiated objects are a child of.
    /// </summary>
    public Transform ActiveRoot { get; private set; }
    /// <summary>
    /// Transform that all inactive GameObjectPool instantiated objects are a child of.
    /// </summary>
    public Transform InactiveRoot { get; private set; }

    private const string c_activeRootName = "Active Game Objects";
    private const string c_inactiveRootName = "Inactive Game Objects";

    private void Start()
    {
        ActiveRoot = new GameObject(c_activeRootName).transform;
        ActiveRoot.SetParent(transform);
        InactiveRoot = new GameObject(c_inactiveRootName).transform;
        InactiveRoot.SetParent(transform);
    }

    /// <summary>
    /// Creates an instance of a game object based on the passed schema prefab. Used by
    /// various GameObjectPools to create new GameObjects if needed.
    /// </summary>
    /// <typeparam name="TGameObjectPoolable">The type of poolable to be created.</typeparam>
    /// <param name="schema">An instance of the poolable to use as a prefab schema for instantiation.</param>
    /// <returns>The newly created instance of the poolable.</returns>
    public TGameObjectPoolable CreateInstance<TGameObjectPoolable>(TGameObjectPoolable schema) where TGameObjectPoolable : MonoBehaviour
    {
        return Instantiate(schema.gameObject).GetComponent<TGameObjectPoolable>();
    }
}
