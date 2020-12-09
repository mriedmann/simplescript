using System.Threading.Tasks;

namespace simplescript.tests.Abstract
{
    internal class CompensateProcedureStepBaseTestExposal<TContextType> : ProcedureStepBaseTestExposal<TContextType>
    {
        public CompensateProcedureStepBaseTestExposal(IStepTrace<TContextType> tracer = null) : base(tracer) { }

        protected async override Task<bool> StepSpecificExecute(TContextType contextType)
        {
            await this.Compensate(contextType);
            return true;
        }
    }
}
