using System;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class InvalidSessionEvent : EventBase
    {
        public Guid SessionId { get; set; }
    }
}