using exampleservice.CustomerService.Contract;

namespace exampleservice.CustomerService
{
    public class SessionContext
    {
        public bool PasswordValid { get; set; }
        public SessionSpecification Session { get; set; }
        public bool WasCompensated { get; set; }
        public SessionCommandBase Command { get; set; }
    }
}