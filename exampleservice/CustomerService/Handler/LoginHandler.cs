using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Utils;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal class LoginHandler : CustomerHandlerBase<LoginCommand>
    {
        public LoginHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository) : base(bus, dataBaseRepository)
        {
        }

        internal override async Task<EventBase> Handle(LoginCommand command)
        {
            this.VerifyIputArguments(command);

            CustomerSpecification customer = await dataBaseRepository.LoadCustomer(command.Username);
            if (customer == null)
            {
                return new LoginFailedEvent() { Username = command.Username };
            }

            string givenPasswordHash = Password.ComputeHash(command.Password);
            if (givenPasswordHash.Equals(customer.PasswordHash))
            {
                SessionSpecification session = SessionSpecification.NewSession();

                if (await dataBaseRepository.SaveSession(session) > 0)
                    return new LoginSucceededEvent() { Username = command.Username, Session = session };
                else
                    return new GenericErrorEvent();
            }
            //invalid password or some other error occured
            return new LoginFailedEvent() { Username = command.Username };
        }

        protected override void VerifyIputArguments(LoginCommand command)
        {
            base.VerifyIputArguments(command);

            if (string.IsNullOrWhiteSpace(command.Username) ||
                string.IsNullOrWhiteSpace(command.Password))
            {
                throw new ArgumentNullException();
            }
        }
    }
}
