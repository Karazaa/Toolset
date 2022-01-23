using UnityEngine;
using System;
using System.Collections;

namespace Toolset.Core
{
    /// <summary>
    /// A Toolset wrapper of MonoBehavior that allows for access to a RoutineRunner
    /// at the ToolsetMonoBehavior level. This is used to make the Routine API as similar
    /// as possible to Unity's Coroutine API
    /// </summary>
    public class ToolsetMonoBehavior : MonoBehaviour, IRoutineManagerService
    {
        private readonly RoutineRunner m_routineRunner = new RoutineRunner();

        protected virtual void Update()
        {
            m_routineRunner.Update();
        }

        protected virtual void FixedUpdate()
        {
            m_routineRunner.FixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            m_routineRunner.LateUpdate();
        }

        public RoutineHandle StartRoutine(IEnumerator routine, Action<Exception> exceptionHandler = null)
        {
            return m_routineRunner.StartRoutine(routine, exceptionHandler);
        }

        public void StopRoutine(RoutineHandle routineHandle)
        {
            m_routineRunner.StopRoutine(routineHandle);
        }

        public void StopAllRoutines()
        {
            m_routineRunner.StopAllRoutines();
        }

        public virtual void Inject(Scope scope)
        {
            // Resolve dependencies if there are any.
        }

        public virtual void Dispose()
        {
            StopAllRoutines();
        }
    }
}
