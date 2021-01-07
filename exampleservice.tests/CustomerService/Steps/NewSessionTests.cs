using exampleservice.Framework.Abstract;
using exampleservice.CustomerService;
using exampleservice.CustomerService.Contract;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using exampleservice.CustomerService.Controller;
using System;
using FluentAssertions.Extensions;

namespace exampleservice.tests.CustomerService.Steps
{
    [TestFixture]
    public class NewSessionTests
    {

        [Test]
        public async Task Execute_Ok()
        {
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.SaveSession(It.IsAny<SessionSpecification>()))
                .ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.NewSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
                context.Session.SessionId.Should().NotBe(Guid.Empty);
                (context.Session.ValidNotAfter - context.Session.CreatedAt).Should().BeCloseTo(new TimeSpan(0, SessionSpecification.DEFAULT_SESSIONTIME, 0), 1.Minutes());
            }
        }
    }
}