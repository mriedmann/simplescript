using System;

namespace exampleservice.CustomerService.Contract
{
    public class SessionSpecification : ICloneable
    {
        public const int DEFAULT_SESSIONTIME = 30;

        public Guid SessionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ValidNotAfter { get; set; }


        public static SessionSpecification NewSession()
        {
            return new SessionSpecification()
            {
                SessionId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                ValidNotAfter = DateTime.Now.AddMinutes(DEFAULT_SESSIONTIME)
            };
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}