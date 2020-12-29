using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class SessionChangedEvent : EventBase
    {
        public SessionSpecification Session {get;set;}
    }
}