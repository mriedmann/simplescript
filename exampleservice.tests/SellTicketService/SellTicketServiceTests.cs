using exampleservice.AccoutingService.Contract;
using exampleservice.Framework.Abstract;
using exampleservice.SellTicketService.Contract;
using exampleservice.SellTicketService.Controller;
using exampleservice.TicketService.Contracts;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace exampleservice.tests.SellTicketService
{
    [TestFixture]
    public class SellTicketServiceTests
    {
        [Test]
        public async Task AllStepSucceed()
        {
            var busMock = new Moq.Mock<IMessageBus>();
            busMock.Setup(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.IsAny<WithdrawFromCustomerCommand>())).
                ReturnsAsync(new WithdrawnFromCustomerEvent());
            busMock.Setup(s => s.RequestAndReply<DepositToCustomerCommand>(It.IsAny<DepositToCustomerCommand>())).
                ReturnsAsync(new DepositedToCustomerEvent());
            busMock.Setup(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.IsAny<FlagTicketAsSoldCommand>())).
                ReturnsAsync(new FlagedTicketAsSoldEvent());
            var dataBaseMock = new Moq.Mock<ISellTicketServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.SaveTicket(It.IsAny<TicketSpecification>())).
                ReturnsAsync(1);
            var instanceUnderTest = new exampleservice.SellTicketService.SellTicketService(busMock.Object, dataBaseMock.Object);
            string ticketNumber = "MyTicketNumber";
            var sellTicketCommand = new SellTicketCommand
            {
                Ticket = new TicketSpecification
                {
                    IssueDate = DateTime.Now.AddDays(-2),
                    TicketNumber = ticketNumber,
                    Price = 1200
                }
            };

            var resultedEvent = await instanceUnderTest.Handle(sellTicketCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(TicketSoldEvent));
                var ticketSoldEvent = (TicketSoldEvent)resultedEvent;
                ticketSoldEvent.TicketNumber.Should().BeSameAs(ticketNumber);
            }
        }

        [Test]
        public async Task SaveTicketFailedSucceed()
        {
            var busMock = new Moq.Mock<IMessageBus>();
            busMock.Setup(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.IsAny<WithdrawFromCustomerCommand>())).
                ReturnsAsync(new WithdrawnFromCustomerEvent());
            busMock.Setup(s => s.RequestAndReply<DepositToCustomerCommand>(It.IsAny<DepositToCustomerCommand>())).
                ReturnsAsync(new DepositedToCustomerEvent());
            busMock.Setup(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.IsAny<FlagTicketAsSoldCommand>())).
                ReturnsAsync(new FlagedTicketAsSoldEvent());
            var dataBaseMock = new Moq.Mock<ISellTicketServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.SaveTicket(It.IsAny<TicketSpecification>())).
                ReturnsAsync(0);
            var instanceUnderTest = new exampleservice.SellTicketService.SellTicketService(busMock.Object, dataBaseMock.Object);
            string ticketNumber = "MyTicketNumber";
            var sellTicketCommand = new SellTicketCommand
            {
                Ticket = new TicketSpecification
                {
                    IssueDate = DateTime.Now.AddDays(-2),
                    TicketNumber = ticketNumber,
                    Price = 1200
                }
            };

            var resultedEvent = await instanceUnderTest.Handle(sellTicketCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CouldNotSellTicketEvent));
                var couldNotSellTicketEvent = (CouldNotSellTicketEvent)resultedEvent;
                couldNotSellTicketEvent.TicketNumber.Should().BeSameAs(ticketNumber);
            }
        }

        [Test]
        public async Task WithdraFromByerFailed()
        {
            var busMock = new Moq.Mock<IMessageBus>();
            busMock.Setup(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.IsAny<WithdrawFromCustomerCommand>())).
                ReturnsAsync(new CouldNotWithdrawFromCustomerEvent());
            var dataBaseMock = new Moq.Mock<ISellTicketServiceDataBaseRepository>();
            var instanceUnderTest = new exampleservice.SellTicketService.SellTicketService(busMock.Object, dataBaseMock.Object);
            string ticketNumber = "MyTicketNumber";
            var sellTicketCommand = new SellTicketCommand
            {
                Ticket = new TicketSpecification
                {
                    IssueDate = DateTime.Now.AddDays(-2),
                    TicketNumber = ticketNumber,
                    Price = 1200
                }
            };

            var resultedEvent = await instanceUnderTest.Handle(sellTicketCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CouldNotSellTicketEvent));
                var couldNotSellTicketEvent = (CouldNotSellTicketEvent)resultedEvent;
                couldNotSellTicketEvent.TicketNumber.Should().BeSameAs(ticketNumber);
            }
            busMock.Verify(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.IsAny<WithdrawFromCustomerCommand>()));
            busMock.VerifyNoOtherCalls();
        }
    }
}
