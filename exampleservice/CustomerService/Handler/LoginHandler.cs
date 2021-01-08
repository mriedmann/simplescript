using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Steps;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using simplescript;
using simplescript.DSL;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal class LoginHandler : CustomerHandlerBase<LoginCommand, SessionContext>
    {
        public LoginHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository) : base(bus, dataBaseRepository)
        {
        }

        internal async override Task<EventBase> Handle(LoginCommand command)
        {
            this.VerifyIputArguments(command);

            var context = new SessionContext() { Command = command };
            await procedure.Value.Execute(context);

            if (context.WasCompensated || !context.PasswordValid)
                return new LoginFailedEvent() { Username = command.Username };

            return new LoginSucceededEvent() { Username = command.Username, Session = context.Session };
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

        protected override Procedure<SessionContext> GetProcedure()
        {
            return ProcedureDescription<SessionContext>.
               Start().
               Then(new CheckCredentials(dataBaseRepository)).
               Then(new NewSession(dataBaseRepository)).
               Then(new SaveSession(dataBaseRepository)).
               Finish();
        }
    }
}
