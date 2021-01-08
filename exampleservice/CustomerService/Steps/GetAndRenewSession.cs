using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Steps
{
    public class GetAndRenewSession : ProcedureStepBase<SessionContext>
    {
        private ICustomerServiceDataBaseRepository dataRepository;
        public GetAndRenewSession(ICustomerServiceDataBaseRepository dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(SessionContext contextType)
        {
            contextType.Session = await this.dataRepository.LoadSession(((CheckSessionCommand)contextType.Command).SessionId);
            if (contextType.Session != null && contextType.Session.ValidNotAfter > DateTime.Now)
            {
                contextType.Session.ValidNotAfter = DateTime.Now.AddMinutes(30);
                return false;
            }

            await this.CompensatePredecssorOnly(contextType);
            contextType.WasCompensated = true;
            return true;
        }
    }
}
