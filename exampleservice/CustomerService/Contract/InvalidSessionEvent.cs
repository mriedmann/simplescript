using System;
using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Contract
{
    public class InvalidSessionEvent : EventBase
    {
        public Guid SessionId { get; set; }
    }
}