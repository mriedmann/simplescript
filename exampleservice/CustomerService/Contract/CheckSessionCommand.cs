using System;

namespace exampleservice.CustomerService.Contract
{
    public class CheckSessionCommand : SessionCommandBase
    {
        public Guid SessionId { get; set; }
    }
}
