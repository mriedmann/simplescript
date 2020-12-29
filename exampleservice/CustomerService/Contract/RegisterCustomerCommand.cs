namespace exampleservice.CustomerService.Contract
{
    public class RegisterCustomerCommand : CustomerServiceCommand
    {
        public CustomerSpecification Customer { get; set; }
    }
}
