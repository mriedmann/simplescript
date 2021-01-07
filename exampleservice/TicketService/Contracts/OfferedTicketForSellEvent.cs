using exampleservice.Framework.BaseFramework;

namespace exampleservice.TicketService.Contracts
{
    public class OfferedTicketForSellEvent : EventBase
    {
        public string TicketNumber { get; internal set; }
    }
}
