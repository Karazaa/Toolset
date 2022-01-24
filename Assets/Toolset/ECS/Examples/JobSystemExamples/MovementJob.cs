using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Toolset.ECS.Examples
{
    public struct MovementJob : IJobParallelForTransform
    {
        public float RotationSpeed { get; set; }
        public float DeltaTime { get; set; }

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 localRotation = transform.localRotation.eulerAngles;
            localRotation.y += RotationSpeed * DeltaTime;
            transform.localRotation = Quaternion.Euler(localRotation);
        }
    }
}