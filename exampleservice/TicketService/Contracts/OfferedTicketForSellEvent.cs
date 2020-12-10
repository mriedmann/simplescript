using exampleservice.Framework.BaseFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace exampleservice.TicketService.Contracts
{
    public class OfferedTicketForSellEvent : EventBase
    {
        public string TicketNumber { get; internal set; }
    }
}
