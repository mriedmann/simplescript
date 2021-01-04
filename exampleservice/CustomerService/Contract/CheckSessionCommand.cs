using System;

namespace exampleservice.CustomerService.Contract
{
    public class CheckSessionCommand : CustomerCommandBase
    {
        public Guid SessionId { get; set; }
    }
}
