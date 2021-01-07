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
    public class GetAndRenewTests
    {
        private Guid testSessionId = new Guid("11111111-1111-1111-1111-111111111111");

        [Test]
        public async Task Execute_SessionExistsAndRepoSaveOk()
        {
            var testSessionSpec = new SessionSpecification()
            {
                SessionId = testSessionId,
                CreatedAt = DateTime.Now.AddMinutes(-5),
                ValidNotAfter = DateTime.Now.AddMinutes(25)
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.LoadSession(testSessionId))
                .ReturnsAsync(testSessionSpec);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.GetAndRenewSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { Command = new CheckSessionCommand { SessionId = testSessionId } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
                context.Session.SessionId.Should().Be(testSessionId);
                (context.Session.ValidNotAfter - context.Session.CreatedAt).Should().BeCloseTo(new TimeSpan(0, SessionSpecification.DEFAULT_SESSIONTIME + 5, 0), 1.Minutes());
            }
        }

        [Test]
        public async Task Execute_NoSession()
        {
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.LoadSession(It.IsAny<Guid>()))
                .ReturnsAsync((SessionSpecification)null);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.GetAndRenewSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { Command = new CheckSessionCommand { SessionId = testSessionId } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
                context.Session.Should().BeNull();
            }
        }

        [Test]
        public async Task Execute_SessionTimedOut()
        {
            var testSessionSpec = new SessionSpecification()
            {
                SessionId = testSessionId,
                CreatedAt = DateTime.Now.AddMinutes(-35),
                ValidNotAfter = DateTime.Now.AddMinutes(-5)
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.LoadSession(testSessionId))
                .ReturnsAsync(testSessionSpec);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.GetAndRenewSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { Command = new CheckSessionCommand { SessionId = testSessionId } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
                context.Session.SessionId.Should().Be(testSessionId);
            }
        }
    }
}