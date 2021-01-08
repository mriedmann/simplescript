using exampleservice.Framework.BaseFramework;
using System;

namespace exampleservice.CustomerService.Contract
{
    public class InvalidSessionEvent : EventBase
    {
        public Guid SessionId { get; set; }
    }
}