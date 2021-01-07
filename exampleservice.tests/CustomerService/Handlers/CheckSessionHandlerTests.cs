using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.Framework.Abstract;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace exampleservice.tests.CustomerService.Handlers
{
    [TestFixture]
    public class CheckSessionHandlerTests
    {
        private Guid testSessionId;
        private SessionSpecification testSession;
        private DateTime now;

        [SetUp]
        public void Setup()
        {
            now = DateTime.Now;
            testSessionId = Guid.NewGuid();
            testSession = new SessionSpecification()
            {
                SessionId = testSessionId,
                CreatedAt = now.AddMinutes(-5),
                ValidNotAfter = now.AddMinutes(25)
            };
        }

        [Test]
        public async Task CheckSessionSucceed()
        {
            testSession.CreatedAt = now.AddMinutes(-5);
            testSession.ValidNotAfter = now.AddMinutes(25);

            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadSession(testSessionId)).
                ReturnsAsync((SessionSpecification)testSession.Clone());
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var checkSessionCommand = new CheckSessionCommand
            {
                SessionId = testSessionId
            };

            var resultedEvent = await instanceUnderTest.Handle(checkSessionCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(SessionChangedEvent));
                var specificResultedEvent = (SessionChangedEvent)resultedEvent;
                specificResultedEvent.Session.SessionId.Should().Be(testSession.SessionId);
                specificResultedEvent.Session.CreatedAt.Should().Be(testSession.CreatedAt);
                specificResultedEvent.Session.ValidNotAfter.Should().BeAfter(testSession.ValidNotAfter);
                // check on relative times to avoid problems with actual times. 5min old session + 30min = 35min overall lifetime
                (specificResultedEvent.Session.ValidNotAfter - specificResultedEvent.Session.CreatedAt).Should().BeCloseTo(new TimeSpan(0, 35, 0), 1.Minutes());
            }
        }

        [Test]
        public async Task CheckSessionFailed_SessionTimeout()
        {
            testSession.CreatedAt = now.AddMinutes(-35);
            testSession.ValidNotAfter = now.AddMinutes(-5);

            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadSession(testSessionId)).
                ReturnsAsync((SessionSpecification)testSession.Clone());
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var checkSessionCommand = new CheckSessionCommand
            {
                SessionId = testSessionId
            };

            var resultedEvent = await instanceUnderTest.Handle(checkSessionCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(InvalidSessionEvent));
                var specificResultedEvent = (InvalidSessionEvent)resultedEvent;
                specificResultedEvent.SessionId.Should().Be(testSession.SessionId);
            }
        }

        [Test]
        public async Task CheckSessionFailed_MissingArgument_SessionId()
        {
            testSession.CreatedAt = now.AddMinutes(-35);
            testSession.ValidNotAfter = now.AddMinutes(-5);

            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadSession(testSessionId)).
                ReturnsAsync((SessionSpecification)testSession.Clone());
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var checkSessionCommand = new CheckSessionCommand
            {
                SessionId = Guid.Empty
            };

            try
            {
                await instanceUnderTest.Handle(checkSessionCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentException was expected but not thrown");

        }
    }
}
