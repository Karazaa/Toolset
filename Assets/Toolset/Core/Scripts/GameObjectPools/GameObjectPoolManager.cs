using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// MonoBehavior Singleton used to facilitate the instantation/organization of GameObjects from various GameObjectPools. This class is
    /// needed as a component of the GameObjectPool implementation pattern since MonoBehaviors cannot be generic, but the pools are generic.
    /// </summary>
    public class GameObjectPoolManager : ToolsetMonoBehaviorSingleton<GameObjectPoolManager>, IGameObjectPoolManagerService
    {
        public Transform ActiveRoot { get; private set; }

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

        public TGameObjectPoolable CreateInstance<TGameObjectPoolable>(TGameObjectPoolable schema) where TGameObjectPoolable : MonoBehaviour
        {
            return Instantiate(schema.gameObject).GetComponent<TGameObjectPoolable>();
        }

        public override void Inject(Scope scope)
        {
            base.Inject(scope);

            // Resolve dependencies if there are any.
        }

        public override void Dispose()
        {
            Destroy(ActiveRoot);
            Destroy(InactiveRoot);

            base.Dispose();
        }
    }
}