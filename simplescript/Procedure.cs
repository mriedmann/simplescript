using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace simplescript
{
    public class Procedure<TContextType>
    {
        private ProcedureStepBase<TContextType> initialStep;

        public Procedure(ProcedureStepBase<TContextType> initialStep)
        {
            this.initialStep = initialStep ?? throw new ArgumentNullException(nameof(initialStep));             
        }

        public async Task Execute(TContextType context)
        {
            await this.initialStep.Execute(context);
        }
    }
}
