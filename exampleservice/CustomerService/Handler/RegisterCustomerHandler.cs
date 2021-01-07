using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Utils;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal class RegisterCustomerHandler : CustomerHandlerBase<RegisterCustomerCommand>
    {

        public RegisterCustomerHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository) : base(bus, dataBaseRepository)
        {
        }

        internal override async Task<EventBase> Handle(RegisterCustomerCommand command)
        {
            this.VerifyIputArguments(command);

            string passwordHash = Password.ComputeHash(command.Customer.Password);
            command.Customer.Password = null; //set PW to null asap to avoid saving or returning cleartext password at all cost

            //check if customer username is already taken
            CustomerSpecification customer = await dataBaseRepository.LoadCustomer(command.Customer.Username);
            if (customer != null)
            {
                return new CustomerRegistrationFailedEvent() { Customer = command.Customer };
            }
            
            command.Customer.PasswordHash = passwordHash;
            command.Customer.CustomerId = Guid.NewGuid();
            
            if (await dataBaseRepository.CreateCustomer(command.Customer) > 0)
            {
                CustomerRegisteredEvent e = new CustomerRegisteredEvent() { Customer = command.Customer };
                await bus.PublishEvent(e);
                return e;
            }

            command.Customer.PasswordHash = null; //set hash to null to be save
            return new GenericErrorEvent();
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
    }
}
