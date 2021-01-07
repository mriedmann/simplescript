using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using simplescript.Abstract;
using System.Threading.Tasks;

namespace simplescript.tests.Abstract
{
    [TestFixture]
    public class ProcedureStepBaseTest
    {
        [Test]
        public async Task Execute_InnerExecutePerformsFlawless_InvokeExecuteOnSuccessor()
        {
            var context = new object();
            var successorMock = new Moq.Mock<ProcedureStepBase<object>>();
            successorMock.Setup(s => s.Execute(It.Is<object>(passed => passed.Equals(context))));
            var instanceUnderTest = new ProcedureStepBaseTestExposal<object>();
            instanceUnderTest.SetSuccessor(successorMock.Object);

            await instanceUnderTest.Execute(context);

            successorMock.VerifyAll();
        }

        [Test]
        public async Task Compensate_StepSpecificCompensatePerformsFlawless_InvokeCompensateOnPredecessor()
        {
            var context = new object();
            var instanceUnderTest = new ProcedureStepBaseTestExposal<object>();
            var predecessorMock = new ProcedureStepBaseTestExposal<object>();
            predecessorMock.SetSuccessor(instanceUnderTest);

            await instanceUnderTest.Compensate(context);

            using (new AssertionScope())
            {
                predecessorMock.CompensateWasCalled.Should().BeTrue();
                predecessorMock.ContextPassedToCompensate.Should().BeSameAs(context);
            }
        }
    }
}
