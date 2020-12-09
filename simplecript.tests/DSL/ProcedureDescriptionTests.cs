using Moq;
using NUnit.Framework;
using simplescript.DSL;
using simplescript.tests.Abstract;
using System.Threading.Tasks;

namespace simplescript.tests.DSL
{
    [TestFixture]
    public class ProcedureDescriptionTests
    {
        [Test]
        public async Task Acceptance()
        {
            ProcedureStepBaseTestExposal<object> firstStep = null; 
            ProcedureStepBaseTestExposal<object> secondStep = null; 
            ProcedureStepBaseTestExposal<object> thirdStep = null;
            var tracerMock = new Moq.Mock<IStepTrace<object>>();
            tracerMock.Setup(
                s => s.Trace(
                        It.Is<ProcedureStepBaseTestExposal<object>>(o => o.Equals(firstStep)),
                        StepTraceActionType.Execute,
                        It.IsAny<object>()));
            tracerMock.Setup(
               s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o.Equals(secondStep)),
                       StepTraceActionType.Execute,
                       It.IsAny<object>()));
            tracerMock.Setup(
               s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o.Equals(thirdStep)),
                       StepTraceActionType.Execute,
                       It.IsAny<object>()));
            var tracerMockInstance = tracerMock.Object;
            firstStep = new ProcedureStepBaseTestExposal<object>(tracerMockInstance); 
            secondStep = new ProcedureStepBaseTestExposal<object>(tracerMockInstance); 
            thirdStep = new ProcedureStepBaseTestExposal<object>(tracerMockInstance);

            var proc = ProcedureDescription<object>.
                Start().
                Then(firstStep).
                Then(secondStep).
                Then(thirdStep).
                Finish();

            await proc.Execute(new object());

            tracerMock.VerifyAll();
        }
    }
}
