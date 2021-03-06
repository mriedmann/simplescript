using exampleservice.CustomerService.Contract;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Controller
{
    public interface ICustomerServiceDataBaseRepository
    {
        Task<int> CreateCustomer(CustomerSpecification customer);

        Task<CustomerSpecification> LoadCustomer(string username);

        Task<int> DeleteCustomer(Guid customerId);

        Task<int> SaveSession(SessionSpecification session);

        Task<SessionSpecification> LoadSession(Guid sessionId);

        Task<int> DeleteSession(Guid sessionId);
    }
}