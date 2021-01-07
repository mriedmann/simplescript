using exampleservice.CustomerService.Controller;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using simplescript;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Handler
{
    internal abstract class CustomerHandlerBase<TCommand, TContext>
    {
        protected IMessageBus bus;
        protected ICustomerServiceDataBaseRepository dataBaseRepository;
        protected Lazy<Procedure<TContext>> procedure;

        public CustomerHandlerBase(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
            this.procedure = new Lazy<Procedure<TContext>>(() => this.GetProcedure());
        }

        internal abstract Task<EventBase> Handle(TCommand command);

        protected virtual void VerifyIputArguments(TCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
        }
        protected abstract Procedure<TContext> GetProcedure();
    }
}