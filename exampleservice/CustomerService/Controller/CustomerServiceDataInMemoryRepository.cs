 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using exampleservice.CustomerService.Contract;

namespace exampleservice.CustomerService.Controller
{
    public class CustomerServiceDataInMemoryRepository : ICustomerServiceDataBaseRepository
    {
        private IDictionary<Guid, CustomerSpecification> customerStore = new Dictionary<Guid, CustomerSpecification>();
        private IDictionary<String, Guid> customerUsernameIndex = new Dictionary<String, Guid>();
        private IDictionary<Guid, SessionSpecification> sessionStore = new Dictionary<Guid, SessionSpecification>();

        public Task<int> CreateCustomer(CustomerSpecification customer)
        {
            if(customerStore.TryAdd(customer.CustomerId, customer)){
                customerUsernameIndex.Add(customer.Username, customer.CustomerId);
                return Task.FromResult(1);
            }
                
            return Task.FromResult(0);
        }

        public Task<CustomerSpecification> LoadCustomer(string username)
        {
            Guid customerId = Guid.Empty;
            if(!customerUsernameIndex.TryGetValue(username, out customerId))
                return Task.FromResult<CustomerSpecification>(null);
            
            CustomerSpecification customer = null;
            if(!customerStore.TryGetValue(customerId, out customer))
                return Task.FromResult<CustomerSpecification>(null);
            
            return Task.FromResult(customer);
        }

        public Task<int> SaveSession(SessionSpecification session)
        {
            if(sessionStore.TryAdd(session.SessionId, session))
                return Task.FromResult(1);
                
            return Task.FromResult(0);
        }

        public Task<SessionSpecification> LoadSession(Guid sessionId)
        {
            SessionSpecification session = null;
            if(sessionStore.TryGetValue(session.SessionId, out session))
                return Task.FromResult(session);
                
            return Task.FromResult<SessionSpecification>(null);
        }

        public Task<int> DeleteCustomer(Guid customerId)
        {
            if(customerStore.Remove(customerId))
                return Task.FromResult(1);
            return Task.FromResult(0);
        }

        public Task<int> DeleteSession(Guid sessionId)
        {
            if(sessionStore.Remove(sessionId))
                return Task.FromResult(1);
            return Task.FromResult(0);
        }
    }
}