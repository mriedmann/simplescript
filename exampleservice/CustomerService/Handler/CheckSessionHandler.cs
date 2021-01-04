using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Events;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
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
            // 1. Session Valid?
            // 2. Renew Session (if necessary)

            this.VerifyIputArguments(command);



            SessionSpecification session = await dataBaseRepository.LoadSession(command.SessionId);

            if (session == null)
                return new InvalidSessionEvent() { SessionId = command.SessionId };



            if (session.ValidNotAfter > DateTime.Now)
            {
                session.ValidNotAfter = DateTime.Now.AddMinutes(30);

                if (await dataBaseRepository.SaveSession(session) > 0)
                    return new SessionChangedEvent() { Session = session };
                else
                    return new GenericErrorEvent();
            }



            return new InvalidSessionEvent() { SessionId = command.SessionId };
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
