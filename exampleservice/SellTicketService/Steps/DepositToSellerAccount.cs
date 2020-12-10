using exampleservice.AccoutingService.Contract;
using exampleservice.Framework.Abstract;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.SellTicketService.Steps
{
    public class DepositToSellerAccount : ProcedureStepBase<SellTicketContext>
    {
        private IMessageBus bus;
        public DepositToSellerAccount(IMessageBus bus) => this.bus = bus ?? throw new ArgumentNullException(nameof(bus));

        protected async override Task<bool> StepSpecificExecute(SellTicketContext contextType)
        {
            var reply = await this.bus.RequestAndReply(
                new DepositToCustomerCommand() 
                { 
                    Amount = contextType.Command.Ticket.Price, 
                    AccountId = contextType.Command.SellerAccountId
                });
            if(reply is CouldNotDepositToCustomerEvent)
            {
                await this.CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                return true;
            }
            else
            {
                contextType.HasDeposit = true;
                return false;
            }
        }

        protected async override Task StepSpecificCompensate(SellTicketContext contextType)
        {
            var reply = await this.bus.RequestAndReply(
                new WithdrawFromCustomerCommand() 
                { 
                    Amount = contextType.Command.Ticket.Price,
                    AccountId = contextType.Command.SellerAccountId
                });
            if (reply is CouldNotWithdrawFromCustomerEvent)
            {
                // TODO notify asbout compensation failed
                // maybe a customer has received unauthorized money
            }
            return;
        }
    }
}
