using System.Collections;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Example class used to validate Scope class through unit tests.
    /// </summary>
    public class ExampleParentScope : Scope
    {
        public IRoutineManagerService GetRoutineManager()
        {
            IRoutineManagerService routineManagerService = null;

            Resolve(ref routineManagerService);
            return routineManagerService;
        }

        public override IEnumerator SetUpScope()
        {
            Register<IRoutineManagerService>(RoutineManager.I);
            yield break;
        }
    }

    /// <summary>
    /// Example class used to validate Scope class through unit tests.
    /// </summary>
    public class ExampleChildScope : Scope
    {
        private IRoutineManagerService m_routineManagerService;
        public IRoutineManagerService RoutineManagerService => m_routineManagerService;

        public override IEnumerator SetUpScope()
        {
            Resolve(ref m_routineManagerService);

            Register<IGameObjectPoolManagerService>(GameObjectPoolManager.I);

            yield break;
        }
    }

    /// <summary>
    /// Example class used to validate Scope class through unit tests.
    /// </summary>
    public class ExampleDuplicateFaultyChildScope : Scope
    {
        public override IEnumerator SetUpScope()
        {
            Register<IGameObjectPoolManagerService>(GameObjectPoolManager.I);
            Register<IGameObjectPoolManagerService>(GameObjectPoolManager.I);

            yield break;
        }
    }

    /// <summary>
    /// Example class used to validate Scope class through unit tests.
    /// </summary>
    public class ExampleNullResolutionFaultyChildScope : Scope
    {
        private IGameObjectPoolManagerService m_gameObjectPoolManagerService;
        public override IEnumerator SetUpScope()
        {
            Resolve(ref m_gameObjectPoolManagerService);
            yield break;
        }
    }
}
