using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using simplescript;
using simplescript.DSL;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace exampleservice.CustomerService
{
    public class CustomerService
    {
        private IMessageBus bus;
        private ICustomerServiceDataBaseRepository dataBaseRepository;

        public CustomerService(IMessageBus bus, ICustomerServiceDataBaseRepository dataBaseRepository)
        {
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
        }

        ///<summary>checks if a session is valid. If it is, return <c>SessionChangedEvent</c>, if it is not return <c>SessionTimeoutEvent</c>.</summary>
        public async Task<EventBase> Handle(CheckSessionCommand command){
            VerifyIputArguments(command);

            SessionSpecification session = await dataBaseRepository.LoadSession(command.SessionId);
            //TODO: Maybe not the best name for the event because this is no timeout
            if(session == null){
                return new SessionTimeoutEvent(){ SessionId = command.SessionId };
            }
            if(session.ValidNotAfter > DateTime.Now){
                session.ValidNotAfter = DateTime.Now.Add(new TimeSpan(0,30,0));
                if(await dataBaseRepository.SaveSession(session) > 0){
                    return new SessionChangedEvent(){Session = session};
                }
                //TODO: report error - db save failed
            }
            return new SessionTimeoutEvent(){ SessionId = command.SessionId };
        }

        ///<summary>handle login requests. If username and password matches, return <c>LoginSucceededEvent</c>, if not return <c>LoginFailedEvent</c>.</summary>
        public async Task<EventBase> Handle(LoginCommand command){
            VerifyIputArguments(command);

            CustomerSpecification customer = await dataBaseRepository.LoadCustomer(command.Username);
            if(customer == null){
                return new LoginFailedEvent(){ Username = command.Username };
            }
            string givenPasswordHash = ComputePasswordHash(command.Password);
            if(givenPasswordHash.Equals(customer.PasswordHash)){
                //TODO: refactor to factory
                Guid sessionId = Guid.NewGuid();
                SessionSpecification session = new SessionSpecification(){
                    SessionId = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    ValidNotAfter = DateTime.Now.Add(new TimeSpan(0,30,0)) //TODO: remove magic number
                };
                if(await dataBaseRepository.SaveSession(session) > 0){
                    return new LoginSucceededEvent(){ Username = command.Username, Session = session };
                }
                //TODO: handle error - db save failed
            }
            //invalid password or some other error occured
            return new LoginFailedEvent(){ Username = command.Username };
        }

        ///<summary>handle user registration requests. Returns <c>CustomerRegisteredEvent</c> on success, returns <c>CustomerRegistrationFailedEvent</c> otherwise (e.g. username is already in use).</summary>
        public async Task<EventBase> Handle(RegisterCustomerCommand command){
            VerifyIputArguments(command);

            string passwordHash = ComputePasswordHash(command.Customer.Password);
            command.Customer.Password = null; //set PW to null asap to avoid saving or returning cleartext password at all cost

            //check if customer username is already taken
            CustomerSpecification customer = await dataBaseRepository.LoadCustomer(command.Customer.Username);
            if(customer != null){
                return new CustomerRegistrationFailedEvent(){Customer = command.Customer};
            }
            //TODO: maybe using the same object for DB and Event is not the best idea. Just copied the pattern from SellTicketService.
            command.Customer.PasswordHash = passwordHash;
            command.Customer.CustomerId = Guid.NewGuid();
            if(await dataBaseRepository.SaveCustomer(command.Customer) > 0){
                return new CustomerRegisteredEvent(){Customer = command.Customer};
            }
            //TODO: handle error - db save failed
            command.Customer.PasswordHash = null; //set hash to null to be save
            return new CustomerRegistrationFailedEvent(){Customer = command.Customer};
        }

        private void VerifyIputArguments(CheckSessionCommand command)
        {
            if(
                command == null ||
                command.SessionId == Guid.Empty
            )
            {
                throw new ArgumentNullException();
            }
        }

        private void VerifyIputArguments(LoginCommand command)
        {
            if(
                command == null || 
                string.IsNullOrWhiteSpace(command.Username) ||
                string.IsNullOrWhiteSpace(command.Password)
            )
            {
                throw new ArgumentNullException();
            }
        }

        private void VerifyIputArguments(RegisterCustomerCommand command)
        {
            //TODO: discuss mandatory attributes
            if(
                command == null ||
                command.Customer == null ||
                command.Customer.Username == null ||
                command.Customer.Password == null
            )
            {
                throw new ArgumentNullException();
            }

            //CustomerId is assigned by the system, no predefinition allowed
            if(command.Customer.CustomerId != Guid.Empty) {
                throw new NotSupportedException();
            }
            //PasswordHash is computed on backend, no predefinition allowed
            if(command.Customer.PasswordHash != null) {
                throw new NotSupportedException();
            }
        }

        //TODO: refactor to utility-class
        public static string ComputePasswordHash(string password)  
        {  
            if(string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
            //From: https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }  
    }
}
