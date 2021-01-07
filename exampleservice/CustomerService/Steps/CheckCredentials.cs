using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Utils;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Steps
{
    public class CheckCredentials : ProcedureStepBase<SessionContext>
    {
        private ICustomerServiceDataBaseRepository dataRepository;
        public CheckCredentials(ICustomerServiceDataBaseRepository dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(SessionContext contextType)
        {
            var customer = await this.dataRepository.LoadCustomer(((LoginCommand)contextType.Command).Username);
            if (customer == null)
            {
                await this.CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                return true;
            }
            else
            {
                var passwordHash = Password.ComputeHash(((LoginCommand)contextType.Command).Password);
                contextType.PasswordValid = (passwordHash == customer.PasswordHash);
                return false;
            }
        }
    }
}
