using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal class CheckSessionHandler : CustomerHandlerBase<CheckSessionCommand>
    {
        private IMessageBus bus;
        private ICustomerServiceDataBaseRepository dataBaseRepository;

        public CheckSessionHandler(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
        }

        internal override async Task<EventBase> Handle(CheckSessionCommand command)
        {
            this.VerifyIputArguments(command);

            SessionSpecification session = await dataBaseRepository.LoadSession(command.SessionId);
            //TODO: Maybe not the best name for the event because this is no timeout
            if (session == null)
            {
                return new SessionTimeoutEvent() { SessionId = command.SessionId };
            }
            if (session.ValidNotAfter > DateTime.Now)
            {
                session.ValidNotAfter = DateTime.Now.AddMinutes(30);
                if (await dataBaseRepository.SaveSession(session) > 0)
                {
                    return new SessionChangedEvent() { Session = session };
                }
                //TODO: report error - db save failed
            }
            return new SessionTimeoutEvent() { SessionId = command.SessionId };
        }

        protected override void VerifyIputArguments(CheckSessionCommand command)
        {
            base.VerifyIputArguments(command);

            if (command.SessionId == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
