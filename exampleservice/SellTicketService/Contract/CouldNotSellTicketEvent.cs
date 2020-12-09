using exampleservice.Framework.BaseFramework;

namespace exampleservice.SellTicketService.Contract
{
    public class CouldNotSellTicketEvent : EventBase
    {
        public string TicketNumber { get; set; }
    }
}
