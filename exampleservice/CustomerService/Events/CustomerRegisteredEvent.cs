using exampleservice.CustomerService.Contract;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class CustomerRegisteredEvent : EventBase
    {
        public CustomerSpecification Customer { get; set; }
    }
}