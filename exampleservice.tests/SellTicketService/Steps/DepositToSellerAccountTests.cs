using exampleservice.AccoutingService.Contract;
using exampleservice.Framework.Abstract;
using exampleservice.SellTicketService;
using exampleservice.SellTicketService.Contract;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace exampleservice.tests.SellTicketService.Steps
{
    [TestFixture]
    public class DepositToSellerAccountTests
    {
        [Test]
        public async Task Execute_AccoutingServiceDepositOk_ReturnOkEvent()
        {
            int price = 1234;
            int sellerAccountId = 13;
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.RequestAndReply<DepositToCustomerCommand>(It.Is<DepositToCustomerCommand>(c => c.Amount == price && c.AccountId == sellerAccountId))).
               ReturnsAsync(new DepositedToCustomerEvent());
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.DepositToSellerAccount(messageBusMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price }, SellerAccountId = sellerAccountId } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.HasDeposit.Should().BeTrue();
                context.WasCompensated.Should().BeFalse();
            }
            messageBusMock.Verify(s => s.RequestAndReply<DepositToCustomerCommand>(It.Is<DepositToCustomerCommand>(c => c.Amount == price && c.AccountId == sellerAccountId)));
        }

        [Test]
        public async Task Execute_AccoutingServiceDepositFails_ReturnFailedEvent()
        {
            int price = 1234;
            int sellerAccountId = 13;
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.RequestAndReply<DepositToCustomerCommand>(It.Is<DepositToCustomerCommand>(c => c.Amount == price && c.AccountId == sellerAccountId))).
               ReturnsAsync(new CouldNotDepositToCustomerEvent());
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.DepositToSellerAccount(messageBusMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price }, SellerAccountId = sellerAccountId } };
            await instanceUnderTest.Execute(context);

            using(new AssertionScope())
            {
                context.HasDeposit.Should().BeFalse();
                context.WasCompensated.Should().BeTrue();
            }
            messageBusMock.Verify(s => s.RequestAndReply<DepositToCustomerCommand>(It.Is<DepositToCustomerCommand>(c => c.Amount == price && c.AccountId == sellerAccountId)));
        }

        [Test]
        public async Task Compensate_AccoutingServiceWithdrawOk_ReturnOkEvent()
        {
            int price = 1234;
            int sellerAccountId = 13;
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.Is<WithdrawFromCustomerCommand>(c => c.Amount == price && c.AccountId == sellerAccountId))).
               ReturnsAsync(new WithdrawnFromCustomerEvent());
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.DepositToSellerAccount(messageBusMock.Object);
            var context = new SellTicketContext { Command =  new SellTicketCommand { Ticket = new TicketSpecification {  Price = price }, SellerAccountId = sellerAccountId } };
            await instanceUnderTest.Compensate(context);

            messageBusMock.Verify(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.Is<WithdrawFromCustomerCommand>(c => c.Amount == price && c.AccountId == sellerAccountId)));
        }
    }
}
