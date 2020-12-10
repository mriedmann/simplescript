using exampleservice.Framework.Abstract;
using exampleservice.TicketService.Contracts;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.SellTicketService.Steps
{
    public class SellTicket : ProcedureStepBase<SellTicketContext>
    {
        private IMessageBus bus;
        public SellTicket(IMessageBus bus) => this.bus = bus ?? throw new ArgumentNullException(nameof(bus));

        protected async override Task<bool> StepSpecificExecute(SellTicketContext contextType)
        {
            var reply = await this.bus.RequestAndReply(new FlagTicketAsSoldCommand { TicketNumber = contextType.Command.Ticket.TicketNumber });
            if(reply is CouldNotFlagTicketAsSoldEvent)
            {
                await this.CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                return true;
            }
            else
            {
                contextType.TicketWasSold = true;
                return false;
            }
        }

        protected async override Task StepSpecificCompensate(SellTicketContext contextType)
        {
            var reply = await this.bus.RequestAndReply(new OfferTicketForSellCommand { TicketNumber = contextType.Command.Ticket.TicketNumber });
            if (reply is CouldNotOfferTicketForSellEvent)
            {
                // TODO notify asbout compensation failed
                // maybe a customer has received unauthorized money
            }
            return;
        }
    }
}
