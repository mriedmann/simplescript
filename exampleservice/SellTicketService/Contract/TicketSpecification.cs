using System;

namespace exampleservice.SellTicketService.Contract
{
    public class TicketSpecification
    {
        public string TicketNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public int Price { get; set; }
    }
}
