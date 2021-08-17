using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// Generic implementation of a MonoBehaviour (GameObject) singleton. Used to create a static accessor 
    /// for an instance of a component on a GameObject that there will only ever be one of.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour that is being made a singleton.</typeparam>
    public class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance;

        /// <summary>
        /// Returns an instance of the MonoBehaviour singleton if it exists already. If not, it creates a new game object flagged DontDestroyOnLoad,
        /// sets it as the instance, and returns it.
        /// </summary>
        public static T I
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject gameObject = new GameObject(nameof(T));
                    s_instance = gameObject.AddComponent<T>();
                    DontDestroyOnLoad(gameObject);
                }
                return s_instance;
            }
        }
    }
}