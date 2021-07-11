using UnityEngine;

/// <summary>
/// Generic implementation of a monobehavior (GameObject) singleton. Used to create a static accessor 
/// for an instance of a component on a GameObject that there will only ever be one of.
/// </summary>
/// <typeparam name="T">The monobehavior that is being made a singleton.</typeparam>
public class MonobehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T k_instance;

    /// <summary>
    /// Returns an instance of the monobehavior singleton if it exists already. If not, it creates a new game object flagged DontDestroyOnLoad,
    /// sets it as the instance, and returns it.
    /// </summary>
    public static T I
    {
        get
        {
            if (k_instance == null)
            {
                GameObject gameObject = new GameObject(nameof(T));
                k_instance = gameObject.AddComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            return k_instance;
        }
    }
}
