using System;
using System.Collections.Generic;
using System.Text;

namespace exampleservice.SellTicketService.Contract
{
    public class TicketSpecification
    {
        public string TicketNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public int Price { get; set; }
    }
}
