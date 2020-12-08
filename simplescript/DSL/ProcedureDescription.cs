using simplescript.Abstract;
using System;
using System.Collections.Generic;

namespace simplescript.DSL
{
    public class ProcedureDescription<TContext>
    {
        private List<ProcedureStepBase<TContext>> steps = new List<ProcedureStepBase<TContext>>();
        private Dictionary<Type, Func<object>> services = new Dictionary<Type, Func<object>>();

        public static ProcedureDescription<TContext> Start()
        {
            return new ProcedureDescription<TContext>();
        }

        public ProcedureDescription<TContext> Then(ProcedureStepBase<TContext> step)
        {
            this.steps.Add(step);
            return this;
        }

        public ProcedureDescription<TContext> UseService<TService>(Func<TService> factory) where TService : class
        {
            services.Add(typeof(TService), factory);
            return this;
        }

        public Procedure<TContext> Finish()
        {
            steps.ForEach(step => this.InjectDependencies(step));
            return new Procedure<TContext>(steps[0]);
        }

        private void InjectDependencies(ProcedureStepBase<TContext> step)
        {
            throw new NotImplementedException();
        }
    }
}
