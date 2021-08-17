using UnityEngine;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// MonoBehavior used in the ExampleSceneGameObjectPools scene to set
    /// a prefab schema for a GameObjectPool used in IntegrationTests/
    /// </summary>
    public class ExampleRunnerGameObjectPool : MonoBehaviour
    {
        [SerializeField]
        private ExamplePoolable m_poolablePrefab = null;

        private void Start()
        {
            GameObjectPool<ExamplePoolable>.I.SetSchema(m_poolablePrefab);
        }
    }
}