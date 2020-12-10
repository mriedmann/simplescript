namespace exampleservice.SellTicketService.Contract
{
    public class SellTicketCommand
    {
        public TicketSpecification Ticket { get; set; }

        public int BuyerAccountId { get; set; }
        public int SellerAccountId { get; set; }
    }
}
