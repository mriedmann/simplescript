namespace exampleservice.AccoutingService.Contract
{
    public class WithdrawFromCustomerCommand
    {
        public int Amount { get; set; }
        public int AccountId { get; internal set; }
    }
}
