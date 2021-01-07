using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Utils;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Steps
{
    public class CreateCustomer : ProcedureStepBase<CustomerContext>
    {
        private ICustomerServiceDataBaseRepository dataRepository;
        public CreateCustomer(ICustomerServiceDataBaseRepository dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(CustomerContext contextType)
        {
            var customer = contextType.Command.Customer;
            customer.PasswordHash = Password.ComputeHash(customer.Password);
            customer.Password = null;
            customer.CustomerId = Guid.NewGuid();
            int rowCount = await this.dataRepository.CreateCustomer(contextType.Command.Customer);
            if (rowCount == 0)
            {
                await this.CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                return true;
            }
            else
            {
                contextType.Customer = customer;
                return false;
            }
        }

        protected async override Task StepSpecificCompensate(CustomerContext contextType)
        {
            if (contextType.Customer != null)
                await this.dataRepository.DeleteCustomer(contextType.Customer.CustomerId);
        }
    }
}
