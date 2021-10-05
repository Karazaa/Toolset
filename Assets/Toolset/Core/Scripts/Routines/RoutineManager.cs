using System;
using System.Collections;
using System.Collections.Generic;
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
            public IEnumerator ParentRoutine { get; private set; }
            public RoutineNode HeadNode { get; set; }
            public Action<Exception> ExceptionHandler { get; set; }

            public RoutineGraph(IEnumerator parentRoutine, Action<Exception> exceptionHandler, RoutineNode headNode)
            {
                ParentRoutine = parentRoutine;
                ExceptionHandler = exceptionHandler;
                HeadNode = headNode;
            }
        }

        private class RoutineNode
        {
            public IEnumerator Routine { get; private set; }
            public RoutineNode ParentNode { get; private set; }

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
                        break;
                    case ToolsetWaitForSeconds toolsetWaitForSeconds:
                        Routine = WaitForSecondsRoutine(toolsetWaitForSeconds.Seconds);
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
        private readonly HashSet<IEnumerator> m_activeParentRoutines = new HashSet<IEnumerator>();

        /// <summary>
        /// Runs the given IEnumerator as if it were a Unity Coroutine, but if an exceptionHandler
        /// is passed, any exceptions thrown by any layer of child IEnumerators will be caught
        /// and passed to the handler.
        /// </summary>
        /// <param name="routine">An IEnumerator for the RoutineManager to run.</param>
        /// <param name="exceptionHandler">
        /// An optional exception handler which will be invoked when exceptions originating
        /// from the routine are caught by RoutineManager
        /// </param>
        public void StartRoutine(IEnumerator routine, Action<Exception> exceptionHandler = null)
        {
            if (!m_activeParentRoutines.Contains(routine))
            {
                m_activeParentRoutines.Add(routine);
                m_outstandingRoutines.Add(new RoutineGraph(routine, exceptionHandler, new RoutineNode(routine, null)));
            }
            else
                Debug.LogWarning("[Toolset.RoutineManager] Attempted to start a routine that is already running! Ignoring second StartRoutine call.");
        }

        private void Update()
        {
            for (int i = m_outstandingRoutines.Count - 1; i >= 0; --i)
            {
                InternalMoveNext(m_outstandingRoutines[i]);

                if (m_outstandingRoutines[i].HeadNode == null)
                {
                    m_activeParentRoutines.Remove(m_outstandingRoutines[i].ParentRoutine);
                    m_outstandingRoutines.RemoveAt(i);
                }
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
                    if (internalRoutine.Current is IEnumerator headRoutine)
                    {
                        routineGraph.HeadNode = new RoutineNode(headRoutine, currentHeadNode);
                        InternalMoveNext(routineGraph);
                    }
                    else if (internalRoutine.Current is YieldInstruction yieldInstruction)
                    {
                        routineGraph.HeadNode = new RoutineNode(yieldInstruction, currentHeadNode);
                        InternalMoveNext(routineGraph);
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