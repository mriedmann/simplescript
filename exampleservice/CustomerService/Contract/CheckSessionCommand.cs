using System;

namespace exampleservice.CustomerService.Contract
{
    public class CheckSessionCommand
    {
        public Guid SessionId { get; set; }
    }
}
