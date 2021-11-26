using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolset.Core.Tests
{
    public class ExampleRoutinesMonoBehavior : MonoBehaviour
    {
        public Coroutine MonoBehaviorCoroutine { get; private set; }
        public int UpdateCount { get; private set; }
        public float FixedDeltaTimeSeconds { get; set; }
        public float CoroutineWaitSeconds { get; set; }

        private bool m_shouldBeCounting;
        private float m_originalFixedDeltaTime;

        public IEnumerator WaitForFixedUpdateRoutine()
        {
            m_originalFixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = FixedDeltaTimeSeconds;
            UpdateCount = 0;
            m_shouldBeCounting = true;

            yield return new WaitForFixedUpdate();

            m_shouldBeCounting = false;
            Time.fixedDeltaTime = m_originalFixedDeltaTime;
        }

        public IEnumerator WaitForEndOfFrameRoutine()
        {
            UpdateCount = 0;
            m_shouldBeCounting = true;

            yield return new WaitForEndOfFrame();

            m_shouldBeCounting = false;
        }

        public IEnumerator WaitForCoroutine()
        {
            MonoBehaviorCoroutine = StartCoroutine(ExampleCoroutine());
            yield return MonoBehaviorCoroutine;
        }

        private IEnumerator ExampleCoroutine()
        {
            yield return new WaitForSecondsRealtime(CoroutineWaitSeconds);
            MonoBehaviorCoroutine = null;
        }

        private void Update()
        {
            if (m_shouldBeCounting)
                UpdateCount = UpdateCount + 1;
        }
    }
}
