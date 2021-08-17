using UnityEngine;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Example poolable MonoBehaviour class used to validate GameObjectPool in Integration tests.
    /// </summary>
    public class ExamplePoolable : MonoBehaviour
    {
        public static int InstanceCount { get; set; }

        private void Start()
        {
            InstanceCount = InstanceCount + 1;
        }
    }

    /// <summary>
    /// Example poolable MonoBehaviour class used to validate GameObjectPool in Integration tests.
    /// </summary>
    public class ExampleFaultyPoolable : MonoBehaviour
    {

    }
}