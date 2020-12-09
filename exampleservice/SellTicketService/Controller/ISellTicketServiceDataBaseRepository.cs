using exampleservice.SellTicketService.Contract;
using System.Threading.Tasks;

namespace exampleservice.SellTicketService.Controller
{
    public interface ISellTicketServiceDataBaseRepository
    {
        Task<int> SaveTicket(TicketSpecification ticket);
    }
}
