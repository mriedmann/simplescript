using exampleservice.CustomerService.Contract;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class LoginSucceededEvent : EventBase
    {
        public string Username { get; set; }
        public SessionSpecification Session { get; set; }
    }
}