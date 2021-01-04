using exampleservice.CustomerService.Contract;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class CustomerRegistrationFailedEvent : EventBase
    {
        public CustomerSpecification Customer { get; set; }
    }
}