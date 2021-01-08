using exampleservice.CustomerService.Controller;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Steps
{
    public class SaveSession : ProcedureStepBase<SessionContext>
    {
        private ICustomerServiceDataBaseRepository dataRepository;
        public SaveSession(ICustomerServiceDataBaseRepository dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(SessionContext contextType)
        {
            int rowCount = await this.dataRepository.SaveSession(contextType.Session);
            if (rowCount == 0)
            {
                await this.CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected async override Task StepSpecificCompensate(SessionContext contextType)
        {
            if (contextType.Session != null)
                await this.dataRepository.DeleteSession(contextType.Session.SessionId);
        }
    }
}
