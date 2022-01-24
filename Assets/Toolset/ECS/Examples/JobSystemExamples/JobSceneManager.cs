using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Toolset.ECS.Examples
{
    public class JobSceneManager : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> m_serializedTransforms = new List<Transform>();
        
        private TransformAccessArray m_transformAccessArray;
        private JobHandle m_jobHandle;

        private void Start()
        {
            m_transformAccessArray = new TransformAccessArray(m_serializedTransforms.ToArray(), -1);
        }

        private void OnDisable()
        {
            m_jobHandle.Complete();
            m_transformAccessArray.Dispose();
        }

        private void Update()
        {
            m_jobHandle.Complete();

            MovementJob movementJob = new MovementJob
            {
                DeltaTime = Time.deltaTime,
                RotationSpeed = 100.0f
            };

            m_jobHandle = movementJob.Schedule(m_transformAccessArray);

            JobHandle.ScheduleBatchedJobs();
        }
    }
}
