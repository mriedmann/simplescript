using exampleservice.Framework.BaseFramework;
using System.Threading.Tasks;

namespace exampleservice.Framework.Abstract
{
    public interface IMessageBus
    {
        Task<EventBase> RequestAndReply<TCommand>(TCommand command);
        Task PublishEvent(EventBase eventToPubslish);
        Task PublishCommand<TCommand>(TCommand commandToPublish);
    }
}
