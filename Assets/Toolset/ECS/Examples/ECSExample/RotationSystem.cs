using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Toolset.ECS.Examples
{
    public class RotationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;

            return Entities.ForEach((ref Rotation rotation, ref RotationSpeed rotationSpeed) =>
            {
                rotation.Value = math.mul(rotation.Value, quaternion.RotateY(deltaTime * rotationSpeed.value));
            }).Schedule(inputDeps);
        }
    }
}
