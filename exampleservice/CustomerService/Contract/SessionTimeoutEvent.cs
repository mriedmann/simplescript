using System;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class SessionTimeoutEvent : EventBase
    {
        public Guid SessionId { get; set; }
    }
}