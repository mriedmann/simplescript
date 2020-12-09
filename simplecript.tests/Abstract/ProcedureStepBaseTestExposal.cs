using simplescript.Abstract;
using System.Threading.Tasks;

namespace simplescript.tests.Abstract
{
    internal class ProcedureStepBaseTestExposal<TContextType> : ProcedureStepBase<TContextType>
    {
        public IStepTrace<TContextType> Tracer { get; set; }

        public bool CompensateWasCalled { get; private set; }
        public TContextType ContextPassedToCompensate { get; private set; }
        public bool ExecuteWasCalled { get; private set; }
        public TContextType ContextPassedToExecute { get; private set; }

        public ProcedureStepBaseTestExposal(IStepTrace<TContextType> tracer = null) => this.Tracer = tracer;

        protected override Task<bool> StepSpecificExecute(TContextType contextType)
        {
            return Task.FromResult(false);
        }

        public override Task Execute(TContextType contextType)
        {
            this.Tracer?.Trace(this, StepTraceActionType.Execute, contextType);
            this.ExecuteWasCalled = true;
            this.ContextPassedToExecute = contextType;
            return base.Execute(contextType);
        }

        public override Task Compensate(TContextType contextType)
        {
            this.Tracer?.Trace(this, StepTraceActionType.Compensate, contextType);
            this.CompensateWasCalled = true;
            this.ContextPassedToCompensate = contextType;
            return base.Compensate(contextType);
        }
    }
}
