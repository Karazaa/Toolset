using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolset.Core
{
    /// <summary>
    /// Class used to run Routines. This is similar to starting Coroutines on MonoBehaviors, but
    /// can handle exceptions thrown by nested IEnumerators.
    /// </summary>
    public class RoutineManager : MonoBehaviorSingleton<RoutineManager>
    {
        private class RoutineGraph
        {
            public RoutineHandle RoutineHandle { get; private set; }
            public RoutineNode HeadNode { get; set; }
            public Action<Exception> ExceptionHandler { get; set; }

            public RoutineGraph(RoutineHandle routineHandle, Action<Exception> exceptionHandler, RoutineNode headNode)
            {
                RoutineHandle = routineHandle;
                ExceptionHandler = exceptionHandler;
                HeadNode = headNode;
            }
        }

        private class RoutineNode
        {
            public IEnumerator Routine { get; private set; }
            public RoutineNode ParentNode { get; private set; }
            public bool IsWaitForFixedUpdate { get; private set; }

            public RoutineNode(IEnumerator routine, RoutineNode parentNode)
            {
                Routine = routine;
                ParentNode = parentNode;
            }

            public RoutineNode(YieldInstruction yieldInstruction, RoutineNode parentNode)
            {
                switch (yieldInstruction)
                {
                    case WaitForEndOfFrame waitForEndOfFrame:
                        Routine = WaitForEndOfFrameRoutine();
                        break;
                    case WaitForFixedUpdate waitForFixedUpdate:
                        Routine = WaitForFixedUpdateRoutine();
                        IsWaitForFixedUpdate = true;
                        break;
                    case ToolsetWaitForSeconds toolsetWaitForSeconds:
                        Routine = WaitForSecondsRoutine(toolsetWaitForSeconds.Seconds);
                        break;
                    case AsyncOperation asyncOperation:
                        Routine = asyncOperation.GetAsIEnumerator();
                        break;
                    case Coroutine coroutine:
                        break;
                    case WaitForSeconds waitForSeconds:
                        throw new InvalidOperationException("[Toolset.RoutineManager] The WaitForSeconds YieldInstruction is not properly supported by RoutineManager. Use ToolsetWaitForSeconds instead!");
                    default:
                        throw new InvalidOperationException("[Toolset.RoutineManager] Encountered YieldInstruction type that is not supported!");
                }
                ParentNode = parentNode;
            }

            private IEnumerator WaitForEndOfFrameRoutine()
            {
                yield return null;
            }

            private IEnumerator WaitForFixedUpdateRoutine()
            {
                yield break;
            }

            private IEnumerator WaitForSecondsRoutine(float seconds)
            {
                float secondsRemaining = seconds;

                DateTime lastIterationTime;
                DateTime currentIterationTime;
                while (secondsRemaining > 0.0f)
                {
                    lastIterationTime = DateTime.Now;
                    yield return null;
                    currentIterationTime = DateTime.Now;
                    secondsRemaining -= ((float)(currentIterationTime - lastIterationTime).TotalSeconds) * Time.timeScale;
                }
            }
        }

        private readonly List<RoutineGraph> m_outstandingRoutines = new List<RoutineGraph>();

        /// <summary>
        /// Runs the given IEnumerator as if it were a Unity Coroutine, but if an exceptionHandler
        /// is passed, any exceptions thrown by any layer of child iterators will be caught
        /// and passed to the handler.
        /// </summary>
        /// <param name="routine">An IEnumerator for the RoutineManager to run.</param>
        /// <param name="exceptionHandler">
        /// An optional exception handler which will be invoked when exceptions originating
        /// from the routine are caught by RoutineManager
        /// </param>
        /// <returns>A RoutineHandle to track the state of the internal state of the Routine in the Routine Manager.</returns>
        public RoutineHandle StartRoutine(IEnumerator routine, Action<Exception> exceptionHandler = null)
        {
            RoutineHandle generatedRoutineHandle = new RoutineHandle();
            m_outstandingRoutines.Add(new RoutineGraph(generatedRoutineHandle, exceptionHandler, new RoutineNode(routine, null)));

            return generatedRoutineHandle;
        }

        private void Update()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                if (!m_outstandingRoutines[i].HeadNode.IsWaitForFixedUpdate)
                    IterateListRoutineAtIndex(i);
            }
        }

        private void FixedUpdate()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                if (m_outstandingRoutines[i].HeadNode.IsWaitForFixedUpdate)
                    IterateListRoutineAtIndex(i);
            }
        }

        private void IterateListRoutineAtIndex(int index)
        {
            InternalMoveNext(m_outstandingRoutines[index]);

            if (m_outstandingRoutines[index].HeadNode == null)
            {

                m_outstandingRoutines.RemoveAt(index);
            }
        }

        private void InternalMoveNext(RoutineGraph routineGraph)
        {
            if (routineGraph.HeadNode == null)
                return;

            RoutineNode currentHeadNode = routineGraph.HeadNode;

            void RemoveHeadNode()
            {
                routineGraph.HeadNode = currentHeadNode.ParentNode;
                InternalMoveNext(routineGraph);
            }

            try
            {
                IEnumerator internalRoutine = currentHeadNode.Routine;
                bool result = internalRoutine.MoveNext();

                if (result)
                {
                    switch (internalRoutine.Current)
                    {
                        case IEnumerator headRoutine:
                            routineGraph.HeadNode = new RoutineNode(headRoutine, currentHeadNode);
                            InternalMoveNext(routineGraph);
                            break;
                        case YieldInstruction yieldInstruction:
                            routineGraph.HeadNode = new RoutineNode(yieldInstruction, currentHeadNode);
                            if (!routineGraph.HeadNode.IsWaitForFixedUpdate)
                                InternalMoveNext(routineGraph);
                            break;
                        case Task task:
                            routineGraph.HeadNode = new RoutineNode(task.GetAsIEnumerator(), currentHeadNode);
                            InternalMoveNext(routineGraph);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    RemoveHeadNode();
                }
            }
            catch (Exception exception)
            {
                routineGraph.ExceptionHandler?.Invoke(exception);
                RemoveHeadNode();
            }
        }
    }
}