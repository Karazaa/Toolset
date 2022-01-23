using System;
using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Integrtion tests to validate the Scope class.
    /// </summary>
    public class TestsScope
    {
        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestScopeCreationDeletion()
        {
            Assert.IsNull(ScopeUtils.CurrentScope);

            yield return ScopeUtils.CreateScope<ExampleParentScope>();

            AssertScope(false);

            yield return ScopeUtils.CreateScope<ExampleChildScope>();

            AssertScope(true);

            yield return ScopeUtils.DestroyScope();

            AssertScope(false);

            yield return ScopeUtils.DestroyScope();

            Assert.IsNull(ScopeUtils.CurrentScope);

            yield return ScopeUtils.DestroyAllScopes();
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestDestroyAllScopes()
        {
            yield return ScopeUtils.CreateScope<ExampleParentScope>();
            yield return ScopeUtils.CreateScope<ExampleChildScope>();

            AssertScope(true);

            yield return ScopeUtils.DestroyAllScopes();

            Assert.IsNull(ScopeUtils.CurrentScope);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestResolution()
        {
            yield return ScopeUtils.CreateScope<ExampleParentScope>();
            yield return ScopeUtils.CreateScope<ExampleChildScope>();

            AssertScope(true);

            Assert.IsNotNull((ScopeUtils.CurrentScope as ExampleChildScope).RoutineManagerService);

            yield return ScopeUtils.DestroyAllScopes();
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestDuplicateRegistration()
        {
            yield return TestFaultyScope(true);
            yield return ScopeUtils.DestroyAllScopes();
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestResolvingNull()
        {
            yield return TestFaultyScope(false);
            yield return ScopeUtils.DestroyAllScopes();
        }

        private IEnumerator TestFaultyScope(bool isDuplicate)
        {
            yield return ScopeUtils.CreateScope<ExampleParentScope>();

            IRoutineManagerService routineManager = (ScopeUtils.CurrentScope as ExampleParentScope).GetRoutineManager();

            bool exceptionThrown = false;
            void ExceptionHandler(Exception exception)
            {
                exceptionThrown = true;
            }

            RoutineHandle handle;
            if (isDuplicate)
            {
                handle = routineManager.StartRoutine(ScopeUtils.CreateScope<ExampleDuplicateFaultyChildScope>(), ExceptionHandler);
            }
            else
            {
                handle = routineManager.StartRoutine(ScopeUtils.CreateScope<ExampleNullResolutionFaultyChildScope>(), ExceptionHandler);
            }

            while (!handle.IsDone)
            {
                yield return null;
            }

            Assert.IsTrue(exceptionThrown);
        }

        private void AssertScope(bool isChildScopeExpected)
        {
            Assert.IsNotNull(ScopeUtils.CurrentScope);

            if (isChildScopeExpected)
                Assert.IsTrue(ScopeUtils.CurrentScope is ExampleChildScope);
            else
                Assert.IsTrue(ScopeUtils.CurrentScope is ExampleParentScope);

        }
    }
}
