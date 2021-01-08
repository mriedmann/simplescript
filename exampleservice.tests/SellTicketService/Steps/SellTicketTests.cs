using exampleservice.Framework.Abstract;
using exampleservice.SellTicketService;
using exampleservice.SellTicketService.Contract;
using exampleservice.TicketService.Contracts;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace exampleservice.tests.SellTicketService.Steps
{
    [TestFixture]
    public class SellTicketTests
    {
        [Test]
        public async Task Execute_TicketServiceOk_ReturnOkEvent()
        {
            int price = 1234;
            string ticketNumber = "MyTicketNumber";
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.Is<FlagTicketAsSoldCommand>(c => c.TicketNumber == ticketNumber))).
               ReturnsAsync(new FlagedTicketAsSoldEvent());
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.SellTicket(messageBusMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price, TicketNumber = ticketNumber } } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.TicketWasSold.Should().BeTrue();
                context.WasCompensated.Should().BeFalse();
            }
            messageBusMock.Verify(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.Is<FlagTicketAsSoldCommand>(c => c.TicketNumber == ticketNumber)));
        }

        [Test]
        public async Task Execute_TicketServiceFails_ReturnFailedEvent()
        {
            int price = 1234;
            string ticketNumber = "MyTicketNumber";
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.Is<FlagTicketAsSoldCommand>(c => c.TicketNumber == ticketNumber))).
               ReturnsAsync(new CouldNotFlagTicketAsSoldEvent());
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.SellTicket(messageBusMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price, TicketNumber = ticketNumber } } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.TicketWasSold.Should().BeFalse();
                context.WasCompensated.Should().BeTrue();
            }
            messageBusMock.Verify(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.Is<FlagTicketAsSoldCommand>(c => c.TicketNumber == ticketNumber)));
        }

        [Test]
        public async Task Compensate_TicketServiceOk_ReturnOkEvent()
        {
            int price = 1234;
            string ticketNumber = "MyTicketNumber";
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.RequestAndReply<OfferTicketForSellCommand>(It.Is<OfferTicketForSellCommand>(c => c.TicketNumber == ticketNumber))).
               ReturnsAsync(new OfferedTicketForSellEvent());
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.SellTicket(messageBusMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price, TicketNumber = ticketNumber } } };
            await instanceUnderTest.Compensate(context);

            messageBusMock.Verify(s => s.RequestAndReply<OfferTicketForSellCommand>(It.Is<OfferTicketForSellCommand>(c => c.TicketNumber == ticketNumber)));
        }
    }
}
