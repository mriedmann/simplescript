using System;

namespace exampleservice.CustomerService.Contract
{
    public class CheckSessionCommand : CustomerServiceCommand
    {
        public Guid SessionId { get; set; }
    }
}
