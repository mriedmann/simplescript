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
    internal class CheckSessionHandler : CustomerHandlerBase<CheckSessionCommand, SessionContext>
    {
        public CheckSessionHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository) : base(bus, dataBaseRepository)
        {
        }

        internal async override Task<EventBase> Handle(CheckSessionCommand command)
        {
            this.VerifyIputArguments(command);

            var context = new SessionContext() { Command = command };
            await procedure.Value.Execute(context);

            if (context.WasCompensated || context.Session == null)
                return new InvalidSessionEvent() { SessionId = command.SessionId };

            return new SessionChangedEvent() { Session = context.Session };
        }

        protected override void VerifyIputArguments(CheckSessionCommand command)
        {
            base.VerifyIputArguments(command);

            if (command.SessionId == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
        }

        protected override Procedure<SessionContext> GetProcedure()
        {
            return ProcedureDescription<SessionContext>.
               Start().
               Then(new GetAndRenewSession(dataBaseRepository)).
               Then(new SaveSession(dataBaseRepository)).
               Finish();
        }
    }
}
