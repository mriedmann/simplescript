using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.CustomerService.Handler;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService
{
    public class CustomerService
    {
        private IMessageBus bus;
        private ICustomerServiceDataBaseRepository dataBaseRepository;

        public CustomerService(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
        }

        public async Task<EventBase> Handle(RegisterCustomerCommand command)
        {
            return await new RegisterCustomerHandler(this.bus, this.dataBaseRepository).Handle(command);
        }

        public async Task<EventBase> Handle(LoginCommand command)
        {
            return await new LoginHandler(this.bus, this.dataBaseRepository).Handle(command);
        }

        public async Task<EventBase> Handle(CheckSessionCommand command)
        {
            return await new CheckSessionHandler(this.bus, this.dataBaseRepository).Handle(command);
        }
    }
}
