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
    public class SaveSessionTests
    {
        private Guid testSessionId = new Guid("11111111-1111-1111-1111-111111111111");

        [Test]
        public async Task Execute_RepoSaveOk()
        {
            var now = DateTime.Now;
            var testSessionSpec = new SessionSpecification()
            {
                SessionId = testSessionId,
                CreatedAt = now,
                ValidNotAfter = now.AddMinutes(SessionSpecification.DEFAULT_SESSIONTIME)
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.SaveSession(It.IsAny<SessionSpecification>()))
                .ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.SaveSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { Command = new CheckSessionCommand { SessionId = testSessionId }, Session = testSessionSpec };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
            }
            dataBaseRepositoryMock.Verify(d => d.SaveSession(It.Is<SessionSpecification>(s => s.SessionId == testSessionId)));
        }

        [Test]
        public async Task Execute_RepoSaveFailed()
        {
            var now = DateTime.Now;
            var testSessionSpec = new SessionSpecification()
            {
                SessionId = testSessionId,
                CreatedAt = now,
                ValidNotAfter = now.AddMinutes(SessionSpecification.DEFAULT_SESSIONTIME)
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.SaveSession(It.IsAny<SessionSpecification>()))
                .ReturnsAsync(0);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.SaveSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { Command = new CheckSessionCommand { SessionId = testSessionId }, Session = testSessionSpec };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
            }
            dataBaseRepositoryMock.Verify(d => d.SaveSession(It.Is<SessionSpecification>(s => s.SessionId == testSessionId)));
        }

        [Test]
        public async Task Componsate()
        {
            var now = DateTime.Now.AddMinutes(-5);
            var testSessionSpec = new SessionSpecification()
            {
                SessionId = testSessionId,
                CreatedAt = now,
                ValidNotAfter = now.AddMinutes(SessionSpecification.DEFAULT_SESSIONTIME)
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.DeleteSession(testSessionId))
                .ReturnsAsync(0);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.SaveSession(dataBaseRepositoryMock.Object);
            var context = new SessionContext { Session = testSessionSpec };
            await instanceUnderTest.Compensate(context);

            dataBaseRepositoryMock.Verify(d => d.DeleteSession(testSessionId));
        }
    }
}