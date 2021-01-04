namespace exampleservice.CustomerService.Contract
{
    public class RegisterCustomerCommand : CustomerCommandBase
    {
        public CustomerSpecification Customer { get; set; }
    }
}
