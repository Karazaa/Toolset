using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Toolset.ECS.Examples
{
    [GenerateAuthoringComponent]
    public struct RotationSpeed : IComponentData
    {
        public float Value { get; set; }
    }
}
