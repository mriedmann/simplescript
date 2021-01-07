using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Steps;
using exampleservice.CustomerService.Utils;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using simplescript;
using simplescript.DSL;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal class RegisterCustomerHandler : CustomerHandlerBase<RegisterCustomerCommand, CustomerContext>
    {
        public RegisterCustomerHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository) : base(bus, dataBaseRepository)
        {
        }

        internal async override Task<EventBase> Handle(RegisterCustomerCommand command)
        {
            this.VerifyIputArguments(command);

            var context = new CustomerContext(){ Command = command }; 
            await procedure.Value.Execute(context);

            context.Command.Customer.Password = null;

            if (context.WasCompensated)
                return new CustomerRegistrationFailedEvent() { Customer = command.Customer };
                 
            return new CustomerRegisteredEvent() { Customer = context.Customer };;
        }

        protected override void VerifyIputArguments(RegisterCustomerCommand command)
        {
            base.VerifyIputArguments(command);

            if (command.Customer == null ||
                command.Customer.Username == null ||
                command.Customer.Password == null)
            {
                throw new ArgumentNullException();
            }

            //CustomerId is assigned by the system, no predefinition allowed
            if (command.Customer.CustomerId != Guid.Empty)
            {
                throw new NotSupportedException();
            }
            //PasswordHash is computed on backend, no predefinition allowed
            if (command.Customer.PasswordHash != null)
            {
                throw new NotSupportedException();
            }
        }

        protected override Procedure<CustomerContext> GetProcedure()
        {
            return ProcedureDescription<CustomerContext>.
               Start().
               Then(new CreateCustomer(dataBaseRepository)).
               Then(new PublishNewCustomerEvent(bus)).
               Finish();
        }
    }
}
