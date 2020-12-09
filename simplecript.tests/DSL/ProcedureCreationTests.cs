using Moq;
using NUnit.Framework;
using simplescript.DSL;
using simplescript.tests.Abstract;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace simplescript.tests.DSL
{
    [TestFixture]
    public class ProcedureCreationTests
    {
        [Test]
        public async Task AllThreeStepsCanExecute()
        {
            ProcedureStepBaseTestExposal<object> firstStep = null;
            ProcedureStepBaseTestExposal<object> secondStep = null;
            ProcedureStepBaseTestExposal<object> thirdStep = null;
            Expression<Action<IStepTrace<object>>>
                firstTraceCall = s => s.Trace(
                        It.Is<ProcedureStepBaseTestExposal<object>>(o => o == firstStep),
                        It.Is<StepTraceActionType>(t => t == StepTraceActionType.Execute),
                        It.IsAny<object>()),
                  secondTraceCall = s => s.Trace(
                        It.Is<ProcedureStepBaseTestExposal<object>>(o => o == secondStep),
                        It.Is<StepTraceActionType>(t => t == StepTraceActionType.Execute),
                        It.IsAny<object>()),
                  thirdTraceCall = s => s.Trace(
                        It.Is<ProcedureStepBaseTestExposal<object>>(o => o == thirdStep),
                        It.Is<StepTraceActionType>(t => t == StepTraceActionType.Execute),
                        It.IsAny<object>());
            var tracerMock = new Moq.Mock<IStepTrace<object>>();
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

            tracerMock.Verify(firstTraceCall);
            tracerMock.Verify(secondTraceCall);
            tracerMock.Verify(thirdTraceCall);
            tracerMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task ThirdStepCompensates()
        {
            ProcedureStepBaseTestExposal<object> firstStep = null;
            ProcedureStepBaseTestExposal<object> secondStep = null;
            ProcedureStepBaseTestExposal<object> thirdStep = null;
            Expression<Action<IStepTrace<object>>>
               firstTraceCallExecute = s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o == firstStep),
                       It.Is<StepTraceActionType>(t => t == StepTraceActionType.Execute),
                       It.IsAny<object>()),
               secondTraceCallExecute = s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o == secondStep),
                       It.Is<StepTraceActionType>(t => t == StepTraceActionType.Execute),
                       It.IsAny<object>()),
               thirdTraceCallExecute = s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o == thirdStep),
                       It.Is<StepTraceActionType>(t => t == StepTraceActionType.Execute),
                       It.IsAny<object>()),
               fourthTraceCallCompensate = s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o == thirdStep),
                       It.Is<StepTraceActionType>(t => t == StepTraceActionType.Compensate),
                       It.IsAny<object>()),
               fifthTraceCallCompensate = s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o == secondStep),
                       It.Is<StepTraceActionType>(t => t == StepTraceActionType.Compensate),
                       It.IsAny<object>()),
               sixtTraceCallCompensate = s => s.Trace(
                       It.Is<ProcedureStepBaseTestExposal<object>>(o => o == firstStep),
                       It.Is<StepTraceActionType>(t => t == StepTraceActionType.Compensate),
                       It.IsAny<object>());
            var tracerMock = new Moq.Mock<IStepTrace<object>>();
            var tracerMockInstance = tracerMock.Object;
            firstStep = new ProcedureStepBaseTestExposal<object>(tracerMockInstance);
            secondStep = new ProcedureStepBaseTestExposal<object>(tracerMockInstance);
            thirdStep = new CompensateProcedureStepBaseTestExposal<object>(tracerMockInstance);

            var proc = ProcedureDescription<object>.
                Start().
                Then(firstStep).
                Then(secondStep).
                Then(thirdStep).
                Finish();

            await proc.Execute(new object());

            tracerMock.Verify(firstTraceCallExecute);
            tracerMock.Verify(secondTraceCallExecute);
            tracerMock.Verify(thirdTraceCallExecute);
            tracerMock.Verify(fourthTraceCallCompensate);
            tracerMock.Verify(fifthTraceCallCompensate);
            tracerMock.Verify(sixtTraceCallCompensate);
            tracerMock.VerifyNoOtherCalls();
        }

    }
}
