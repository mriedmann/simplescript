using System;

namespace exampleservice.CustomerService.Contract
{
    public class SessionSpecification : ICloneable
    {
        public Guid SessionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ValidNotAfter { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}