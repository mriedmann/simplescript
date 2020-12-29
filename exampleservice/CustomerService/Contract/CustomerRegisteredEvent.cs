using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class CustomerRegisteredEvent : EventBase
    {
        public CustomerSpecification Customer {get;set;}
    }
}