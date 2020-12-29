using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class LoginSucceededEvent : EventBase
    {
        public string Username { get; set; }
        public SessionSpecification Session { get; set; }
    }
}