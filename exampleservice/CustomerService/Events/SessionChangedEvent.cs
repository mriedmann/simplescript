using exampleservice.CustomerService.Contract;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class SessionChangedEvent : EventBase
    {
        public SessionSpecification Session { get; set; }
    }
}