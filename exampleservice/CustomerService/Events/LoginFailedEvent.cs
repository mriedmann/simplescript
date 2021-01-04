using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class LoginFailedEvent : EventBase
    {
        public string Username { get; set; }
    }
}