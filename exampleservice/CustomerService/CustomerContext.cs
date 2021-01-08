using exampleservice.CustomerService.Contract;

namespace exampleservice.CustomerService
{
    public class CustomerContext
    {
        public CustomerSpecification Customer { get; set; }
        public bool WasCompensated { get; set; }
        public RegisterCustomerCommand Command { get; set; }
    }
}