using exampleservice.SellTicketService.Contract;

namespace exampleservice.SellTicketService
{
    public class SellTicketContext
    {
        public bool WasCompensated { get; set; }

        public SellTicketCommand Command { get; set; }
        public bool HasDeposit { get; internal set; }
        public bool HasWithdrawn { get; set; }
    }
}
