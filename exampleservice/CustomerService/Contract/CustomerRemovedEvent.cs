using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class CustomerRemovedEvent : EventBase
    {
        public CustomerSpecification Customer { get; set; }
    }
}