using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolset.Core
{
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
        }

        private readonly List<RoutineGraph> m_outstandingRoutines = new List<RoutineGraph>();
        private readonly HashSet<IEnumerator> m_activeParentRoutines = new HashSet<IEnumerator>();

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