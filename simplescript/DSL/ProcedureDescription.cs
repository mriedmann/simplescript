using simplescript.Abstract;
using System;

namespace simplescript.DSL
{
    public class ProcedureDescription<TContext>
    {
        private ProcedureStepBase<TContext> initStep;
        private ProcedureStepBase<TContext> lastAddedStep;

        public static ProcedureDescription<TContext> Start()
        {
            return new ProcedureDescription<TContext>();
        }

        public ProcedureDescription<TContext> Then(ProcedureStepBase<TContext> step)
        {
            if (this.initStep == null)
            {
                this.initStep = step;
            }
            if (this.lastAddedStep != null)
            {
                this.lastAddedStep.SetSuccessor(step);
            }
            this.lastAddedStep = step;
            return this;
        }

        public ProcedureDescription<TContext> UseService<TService>(Func<TService> factory) where TService : class
        {
            throw new NotSupportedException("IOC capabilities will be implemented by upcoming release.");
        }

        public Procedure<TContext> Finish()
        {
            return new Procedure<TContext>(this.initStep);
        }
    }
}
