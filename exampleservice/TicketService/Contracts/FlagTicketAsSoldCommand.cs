using System;
using System.Collections.Generic;
using System.Text;

namespace exampleservice.TicketService.Contracts
{
    public class FlagTicketAsSoldCommand
    {
        public string TicketNumber { get; internal set; }
    }
}
