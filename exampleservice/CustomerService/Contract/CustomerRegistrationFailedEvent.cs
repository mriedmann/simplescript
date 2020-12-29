using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class CustomerRegistrationFailedEvent : EventBase
    {
        public CustomerSpecification Customer { get; set; }
    }
}