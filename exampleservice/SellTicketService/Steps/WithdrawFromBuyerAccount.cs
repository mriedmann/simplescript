using exampleservice.AccoutingService.Contract;
using exampleservice.Framework.Abstract;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.SellTicketService.Steps
{
    public class WithdrawFromBuyerAccount : ProcedureStepBase<SellTicketContext>
    {
        private IMessageBus bus;
        public WithdrawFromBuyerAccount(IMessageBus bus) => this.bus = bus ?? throw new ArgumentNullException(nameof(bus));

        protected async override Task<bool> StepSpecificExecute(SellTicketContext contextType)
        {
            var reply = await this.bus.RequestAndReply(new WithdrawFromCustomerCommand());
            if (reply is CouldNotWithdrawFromCustomerEvent)
            {
                await this.CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected async override Task StepSpecificCompensate(SellTicketContext contextType)
        {
            var reply = await this.bus.RequestAndReply(new DepositToCustomerCommand());
            if (reply is CouldNotDepositToCustomerEvent)
            {
                // TODO notify asbout compensation failed
                // maybe a customer has not got his money
            }
            return;
        }
    }
}
