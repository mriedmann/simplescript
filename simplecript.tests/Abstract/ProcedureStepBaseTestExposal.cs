using simplescript.Abstract;
using System.Threading.Tasks;

namespace simplescript.tests.Abstract
{
    internal class ProcedureStepBaseTestExposal<TContextType> : ProcedureStepBase<TContextType>
    {
        private IStepTrace<TContextType> tracer;

        public bool CompensateWasCalled { get; private set; }
        public TContextType ContextPassedToCompensate { get; private set; }
        public bool ExecuteWasCalled { get; private set; }
        public TContextType ContextPassedToExecute { get; private set; }

        public ProcedureStepBaseTestExposal(IStepTrace<TContextType> tracer = null) => this.tracer = tracer;

        protected override Task StepSpecificExecute(TContextType contextType)
        {
            return Task.CompletedTask;
        }

        public override Task Execute(TContextType contextType)
        {
            this.tracer?.Trace(this, StepTraceActionType.Execute, contextType);
            this.ExecuteWasCalled = true;
            this.ContextPassedToExecute = contextType;
            return base.Execute(contextType);
        }

        public override Task Compensate(TContextType contextType)
        {
            this.tracer?.Trace(this, StepTraceActionType.Compensate, contextType);
            this.CompensateWasCalled = true;
            this.ContextPassedToCompensate = contextType;
            return base.Compensate(contextType);
        }
    }
}
