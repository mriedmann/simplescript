using System;

namespace exampleservice.CustomerService.Contract
{
    public class CustomerSpecification : ICloneable
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DayOfBirth { get; set; }

        public string Phonenumber { get; set; }

        public string Username { get; set; }

        ///<remark>deliberately NOT using SecureString here (also not available on .NET Core)</remark>
        ///<see>https://github.com/dotnet/platform-compat/blob/master/docs/DE0001.md</see>
        public string Password { get; set; }

        public string PasswordHash { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
