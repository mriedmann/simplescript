using simplescript.Abstract;
using System;

namespace simplescript.DSL
{
    public class ProcedureDescription<TContext>
    {
        public static ProcedureDescription<TContext> Start()
        {
            throw new NotImplementedException();
        }

        public ProcedureDescription<TContext> Then(ProcedureStepBase<TContext> context)
        {
            throw new NotImplementedException();
        }

        public ProcedureDescription<TContext> UseService<TService>(Func<TService> factory)
        {
            throw new NotImplementedException();
        }

        public Procedure<TContext> Finish()
        {
            throw new NotImplementedException();
        }
    }
}
