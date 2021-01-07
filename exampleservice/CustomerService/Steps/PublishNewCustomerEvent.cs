using exampleservice.CustomerService.Contract;
using exampleservice.Framework.Abstract;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Steps
{
    public class PublishNewCustomerEvent : ProcedureStepBase<CustomerContext>
    {
        private IMessageBus bus;
        public PublishNewCustomerEvent(IMessageBus bus) => this.bus = bus ?? throw new ArgumentNullException(nameof(bus));

        protected async override Task<bool> StepSpecificExecute(CustomerContext contextType)
        {
            CustomerRegisteredEvent e = new CustomerRegisteredEvent() { Customer = contextType.Customer };
            await bus.PublishEvent(e);
            return false;
        }

        protected async override Task StepSpecificCompensate(CustomerContext contextType)
        {
            CustomerRemovedEvent e = new CustomerRemovedEvent() { Customer = contextType.Customer };
            await bus.PublishEvent(e);
        }
    }
}

