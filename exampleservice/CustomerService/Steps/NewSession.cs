using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Steps
{
    public class NewSession : ProcedureStepBase<SessionContext>
    {
        private ICustomerServiceDataBaseRepository dataRepository;
        public NewSession(ICustomerServiceDataBaseRepository dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(SessionContext contextType)
        {
            contextType.Session = SessionSpecification.NewSession();
            return await Task.FromResult(true);
        }
    }
}
