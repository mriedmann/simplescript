using exampleservice.SellTicketService.Contract;

namespace exampleservice.SellTicketService
{
    public class SellTicketContext
    {
        public bool WasCompensated { get; set; }

        public SellTicketCommand Command { get; set; }
    }
}
