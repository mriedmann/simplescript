using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Utils;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal class LoginHandler : CustomerHandlerBase<LoginCommand>
    {
        private IMessageBus bus;
        private ICustomerServiceDataBaseRepository dataBaseRepository;

        public LoginHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
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
                //TODO: refactor to factory
                Guid sessionId = Guid.NewGuid();
                SessionSpecification session = new SessionSpecification()
                {
                    SessionId = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    ValidNotAfter = DateTime.Now.AddMinutes(30) //TODO: remove magic number
                };
                if (await dataBaseRepository.SaveSession(session) > 0)
                {
                    return new LoginSucceededEvent() { Username = command.Username, Session = session };
                }
                //TODO: handle error - db save failed
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
