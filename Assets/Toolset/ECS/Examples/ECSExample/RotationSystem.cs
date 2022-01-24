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
    [AlwaysSynchronizeSystem]
    public class RotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Rotation rotation, ref RotationSpeed rotationSpeed) =>
            {
                rotation.Value = math.mul(rotation.Value, quaternion.RotateY(deltaTime * rotationSpeed.Value));
            }).Run();
        }
    }
}
