using exampleservice.CustomerService.Controller;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal abstract class CustomerHandlerBase<T> where T : CustomerCommandBase
    {
        protected IMessageBus bus;
        protected ICustomerServiceDataBaseRepository dataBaseRepository;

        public CustomerHandlerBase(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
        }

        internal abstract Task<EventBase> Handle(T command);

        protected virtual void VerifyIputArguments(T command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
        }

    }
}