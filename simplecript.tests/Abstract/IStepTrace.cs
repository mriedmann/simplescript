using simplescript.Abstract;

namespace simplescript.tests.Abstract
{
    public interface IStepTrace<TContext>
    {
        void Trace(ProcedureStepBase<TContext> step, StepTraceActionType traceAction, TContext context);
    }
}
