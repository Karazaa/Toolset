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
    public class RoutineRunner
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
                        Routine = null;
                        IsWaitForFixedUpdate = true;
                        break;

                    case ToolsetWaitForSeconds toolsetWaitForSeconds:
                        Routine = WaitForSecondsRoutine(toolsetWaitForSeconds.Seconds);
                        break;

                    case AsyncOperation asyncOperation:
                        Routine = asyncOperation.GetAsIEnumerator();
                        break;

                    case Coroutine coroutine:
                        throw new InvalidOperationException("[Toolset.RoutineRunner] The Coroutine YieldInstruction is not properly supported by RoutineRunner. Use RoutineHandles instead!");

                    case WaitForSeconds waitForSeconds:
                        throw new InvalidOperationException("[Toolset.RoutineRunner] The WaitForSeconds YieldInstruction is not properly supported by RoutineRunner. Use ToolsetWaitForSeconds instead!");
                    
                    default:
                        throw new InvalidOperationException("[Toolset.RoutineRunner] Encountered YieldInstruction type that is not supported!");
                }
                ParentNode = parentNode;
            }

            // NOTE: Since WaitForEndOfFrame is not supported when the UnityEditor is run in batch mode, Toolset
            // will convert any instances of WaitForEndOfFrame to 'yield return null' when encountered in the RoutineRunner.
            private IEnumerator WaitForEndOfFrameRoutine()
            {
                yield return null;
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
        /// <param name="routine">An IEnumerator for the RoutineRunner to run.</param>
        /// <param name="exceptionHandler">
        /// An optional exception handler which will be invoked when exceptions originating
        /// from the routine are caught by RoutineRunner
        /// </param>
        /// <returns>A RoutineHandle to track the state of the internal state of the Routine in the Routine Manager.</returns>
        public RoutineHandle StartRoutine(IEnumerator routine, Action<Exception> exceptionHandler = null)
        {
            RoutineHandle generatedRoutineHandle = new RoutineHandle();
            m_outstandingRoutines.Add(new RoutineGraph(generatedRoutineHandle, exceptionHandler, new RoutineNode(routine, null)));
            InternalMoveNext(m_outstandingRoutines[m_outstandingRoutines.Count - 1]);

            return generatedRoutineHandle;
        }

        /// <summary>
        /// Stops the routine in the RoutineRunner that is associated with the passed RoutineHandle.
        /// </summary>
        /// <param name="routineHandle">The handle object of the routine to stop.</param>
        public void StopRoutine(RoutineHandle routineHandle)
        {
            int index = m_outstandingRoutines.FindIndex((routineGraph) => 
            {
                return routineGraph.RoutineHandle == routineHandle;
            });

            if (index == -1)
                return;

            MarkRoutineStopped(m_outstandingRoutines[index]);
        }

        /// <summary>
        /// Stops all currently running routines in the RoutineRunner.
        /// </summary>
        public void StopAllRoutines()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                MarkRoutineStopped(m_outstandingRoutines[i]);
            }
        }

        internal void Update()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                if (m_outstandingRoutines[i].HeadNode != null && !m_outstandingRoutines[i].HeadNode.IsWaitForFixedUpdate)
                    InternalMoveNext(m_outstandingRoutines[i]);
            }
        }

        internal void FixedUpdate()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                if (m_outstandingRoutines[i].HeadNode != null && m_outstandingRoutines[i].HeadNode.IsWaitForFixedUpdate)
                {
                    RemoveHeadNode(m_outstandingRoutines[i]);
                    InternalMoveNext(m_outstandingRoutines[i]);
                }
            }
        }

        internal void LateUpdate()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                if (m_outstandingRoutines[i].HeadNode == null)
                    m_outstandingRoutines.RemoveAt(i);
            }
        }

        private void InternalMoveNext(RoutineGraph routineGraph)
        {
            if (routineGraph.HeadNode == null)
                return;

            try
            {
                if (ShouldSkipIteration(routineGraph))
                    return;

                if (routineGraph.HeadNode.Routine.MoveNext())
                {
                    switch (routineGraph.HeadNode.Routine.Current)
                    {
                        case IEnumerator headRoutine:
                            routineGraph.HeadNode = new RoutineNode(headRoutine, routineGraph.HeadNode);
                            InternalMoveNext(routineGraph);
                            return;

                        case YieldInstruction yieldInstruction:
                            routineGraph.HeadNode = new RoutineNode(yieldInstruction, routineGraph.HeadNode);
                            if (!routineGraph.HeadNode.IsWaitForFixedUpdate)
                                InternalMoveNext(routineGraph);
                            return;

                        case Task task:
                            routineGraph.HeadNode = new RoutineNode(task.GetAsIEnumerator(), routineGraph.HeadNode);
                            InternalMoveNext(routineGraph);
                            return;

                        case RoutineHandle routineHandle:
                            if (routineHandle.IsDone)
                                InternalMoveNext(routineGraph);
                            return;

                        default:
                            return;
                    }
                }
                else
                {
                    RemoveHeadNode(routineGraph);
                    InternalMoveNext(routineGraph);
                }
            }
            catch (Exception exception)
            {
                RemoveHeadNode(routineGraph);
                routineGraph.ExceptionHandler?.Invoke(exception);
                InternalMoveNext(routineGraph);
            }
        }

        private bool ShouldSkipIteration(RoutineGraph routineGraph)
        {
            return routineGraph.HeadNode.Routine.Current is RoutineHandle preIterationHandle && !preIterationHandle.IsDone;
        }

        private void RemoveHeadNode(RoutineGraph routineGraph)
        {
            routineGraph.HeadNode = routineGraph.HeadNode.ParentNode;

            if (routineGraph.HeadNode == null)
                routineGraph.RoutineHandle.IsDone = true;
        }

        private void MarkRoutineStopped(RoutineGraph routineGraph)
        {
            routineGraph.HeadNode = null;
            routineGraph.RoutineHandle.IsDone = true;
        }
    }
}