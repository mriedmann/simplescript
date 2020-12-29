using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class LoginFailedEvent : EventBase
    {
        public string Username { get; set; }
    }
}