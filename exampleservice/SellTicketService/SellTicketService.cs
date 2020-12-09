using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using exampleservice.SellTicketService.Contract;
using exampleservice.SellTicketService.Controller;
using exampleservice.SellTicketService.Steps;
using simplescript;
using simplescript.DSL;
using System;
using System.Threading.Tasks;

namespace exampleservice.SellTicketService
{
    public class SellTicketService
    {
        private Lazy<Procedure<SellTicketContext>> procedure;
        private IMessageBus bus;
        private ISellTicketServiceDataBaseRepository dataBaseRepository;

        public SellTicketService(IMessageBus bus, ISellTicketServiceDataBaseRepository dataBaseRepository)
        {
            procedure = new Lazy<Procedure<SellTicketContext>>(() => this.GetProcedure());
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
        }

        public async Task<EventBase> Handle(SellTicketCommand command)
        {
            this.VerifyIputArguments(command);
            var context = new SellTicketContext() { Command = command };
            await procedure.Value.Execute(context);
            if (context.WasCompensated)
            {
                return new CouldNotSellTicketEvent { TicketNumber = command.Ticket.TicketNumber }; 
            }
            else
            {
                return new TicketSoldEvent { TicketNumber = command.Ticket.TicketNumber };
            }
        }

        private void VerifyIputArguments(SellTicketCommand command)
        {
            if(command == null)
            {
                throw new ArgumentNullException();
            }
        }

        private Procedure<SellTicketContext> GetProcedure()
        {
            var withdrawFromBuyerStep = new WithdrawFromBuyerAccount(this.bus);
            var depositToSellerStep = new DepositToSellerAccount(this.bus);
            var sellTicketStep = new SellTicket(this.bus);
            var saveSoldTicketStep = new SaveSoldTicketWithOptimisticLock(this.dataBaseRepository);
            return ProcedureDescription<SellTicketContext>.
               Start().
               Then(withdrawFromBuyerStep).
               Then(depositToSellerStep).
               Then(sellTicketStep).
               Then(saveSoldTicketStep).
               Finish();
        }
    }
}
