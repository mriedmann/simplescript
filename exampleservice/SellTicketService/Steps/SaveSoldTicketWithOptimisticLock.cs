using exampleservice.SellTicketService.Controller;
using simplescript.Abstract;
using System;
using System.Threading.Tasks;

namespace exampleservice.SellTicketService.Steps
{
    public class SaveSoldTicketWithOptimisticLock : ProcedureStepBase<SellTicketContext>
    {
        private ISellTicketServiceDataBaseRepository dataRepository;
        public SaveSoldTicketWithOptimisticLock(ISellTicketServiceDataBaseRepository dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(SellTicketContext contextType)
        {
            int rowCount = await this.dataRepository.SaveTicket(contextType.Command.Ticket);
            if(rowCount == 0 )
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
    }
}
