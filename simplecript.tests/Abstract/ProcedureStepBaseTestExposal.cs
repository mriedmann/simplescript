using simplescript.Abstract;
using System.Threading.Tasks;

namespace simplescript.tests.Abstract
{
    internal class ProcedureStepBaseTestExposal<TContextType> : ProcedureStepBase<TContextType>
    {
        public bool CompensateWasCalled { get; private set; }

        public TContextType ContextPassedToCompensate { get; private set; }

        protected override Task StepSpecificExecute(TContextType contextType)
        {
            return Task.CompletedTask;
        }

        public override Task Compensate(TContextType contextType)
        {
            this.CompensateWasCalled = true;
            this.ContextPassedToCompensate = contextType;
            return base.Compensate(contextType);
        }
    }
}
