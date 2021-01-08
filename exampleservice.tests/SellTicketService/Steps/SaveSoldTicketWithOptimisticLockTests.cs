using exampleservice.SellTicketService;
using exampleservice.SellTicketService.Contract;
using exampleservice.SellTicketService.Controller;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace exampleservice.tests.SellTicketService.Steps
{
    [TestFixture]
    public class SaveSoldTicketWithOptimisticLockTests
    {
        [Test]
        public async Task Execute_DataBaseUpdateSucceeded_PerformsFlawless()
        {
            int price = 1234;
            string ticketNumber = "MyTicketNumber";
            DateTime issueDate = DateTime.Now.AddDays(-1);
            var dataBaseRepositoryMock = new Moq.Mock<ISellTicketServiceDataBaseRepository>();
            dataBaseRepositoryMock.Setup(s => s.SaveTicket(It.Is<TicketSpecification>(c => c.TicketNumber == ticketNumber))).
               ReturnsAsync(1);
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.SaveSoldTicketWithOptimisticLock(dataBaseRepositoryMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price, TicketNumber = ticketNumber, IssueDate = issueDate } } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
            }
            dataBaseRepositoryMock.Verify(s => s.SaveTicket(It.Is<TicketSpecification>(c => c.TicketNumber == ticketNumber)));
        }

        [Test]
        public async Task Execute_DataBaseUpdateFails_CompensationStarted()
        {
            int price = 1234;
            string ticketNumber = "MyTicketNumber";
            DateTime issueDate = DateTime.Now.AddDays(-1);
            var dataBaseRepositoryMock = new Moq.Mock<ISellTicketServiceDataBaseRepository>();
            dataBaseRepositoryMock.Setup(s => s.SaveTicket(It.Is<TicketSpecification>(c => c.TicketNumber == ticketNumber))).
               ReturnsAsync(0);
            var instanceUnderTest = new exampleservice.SellTicketService.Steps.SaveSoldTicketWithOptimisticLock(dataBaseRepositoryMock.Object);
            var context = new SellTicketContext { Command = new SellTicketCommand { Ticket = new TicketSpecification { Price = price, TicketNumber = ticketNumber, IssueDate = issueDate } } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
            }
            dataBaseRepositoryMock.Verify(s => s.SaveTicket(It.Is<TicketSpecification>(c => c.TicketNumber == ticketNumber)));
        }
    }
}
