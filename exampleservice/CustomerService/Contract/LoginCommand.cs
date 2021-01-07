namespace exampleservice.CustomerService.Contract
{
    public class LoginCommand : SessionCommandBase
    {
        public string Username { get; set; }

        ///<remark>deliberately NOT using SecureString here (also not available on .NET Core)</remark>
        ///<see>https://github.com/dotnet/platform-compat/blob/master/docs/DE0001.md</see>
        public string Password { get; set; }
    }
}
