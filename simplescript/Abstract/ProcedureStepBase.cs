using System.Threading.Tasks;

namespace simplescript.Abstract
{
    public abstract class ProcedureStepBase<TContextType>
    {
        protected ProcedureStepBase<TContextType> successor;
        protected ProcedureStepBase<TContextType> predecessor;

        public ProcedureStepBase() {}

        private void SetPredecessor(ProcedureStepBase<TContextType> predecessor)
        {
            this.predecessor = predecessor;
        }

        internal void SetSuccessor(ProcedureStepBase<TContextType> successor)
        {
            this.successor = successor;
            successor.SetPredecessor(this);
        }

        protected abstract Task StepSpecificExecute(TContextType contextType);
     
        protected virtual Task StepSpecificCompensate(TContextType contextType)  { return Task.CompletedTask; }

        public async virtual Task Execute(TContextType contextType)
        {
            await this.StepSpecificExecute(contextType);
            if (this.successor != null)
            {
                await this.successor.Execute(contextType);
            }
        }

        public async virtual Task Compensate(TContextType contextType)
        {
            await this.StepSpecificCompensate(contextType);
            if (this.predecessor != null)
            {
                await this.predecessor.Compensate(contextType);
            }
        }
    }
}
