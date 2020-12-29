using exampleservice.AccoutingService.Contract;
using exampleservice.Framework.Abstract;
using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.TicketService.Contracts;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace exampleservice.tests.CustomerService
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private const string testCustomerUsername = "testuser1";
        private const string testCustomerPassword = "password1234";
        private CustomerSpecification testCustomerSpec;
        private CustomerSpecification registerCustomerSpec;
        
        [SetUp]
        public void Setup(){
            testCustomerSpec = new CustomerSpecification()
            {
                CustomerId = Guid.NewGuid(),
                FirstName = "FN",
                LastName = "LN",
                Username = testCustomerUsername,
                Phonenumber = "+123456789",
                DayOfBirth = new DateTime(1999, 9, 9),
                Password = null,
                PasswordHash = exampleservice.CustomerService.CustomerService.ComputePasswordHash(testCustomerPassword)
            };

            registerCustomerSpec = new CustomerSpecification()
            {
                Username = testCustomerUsername,
                Password = "otherPassword1234",
                FirstName = "otherFN",
                LastName = "otherLN",
                Phonenumber = "+987654321",
                DayOfBirth = new DateTime(2002, 2, 2)
            };
        }

        [Test]
        public async Task LoginSucceed()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var loginCommand = new LoginCommand
            {
                Username = testCustomerSpec.Username,
                Password = testCustomerPassword
            };

            var resultedEvent = await instanceUnderTest.Handle(loginCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(LoginSucceededEvent));
                var specificResultedEvent = (LoginSucceededEvent)resultedEvent;
                specificResultedEvent.Username.Should().Be(testCustomerSpec.Username);
                specificResultedEvent.Session.SessionId.Should().NotBe(Guid.Empty);
                specificResultedEvent.Session.ValidNotAfter.Should().BeAfter(DateTime.Now.Add(new TimeSpan(0, 25, 0))); //TODO: flacky test, should not rely on timing
            }
        }

        [Test]
        public async Task LoginFailed_WrongPassword()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var loginCommand = new LoginCommand
            {
                Username = testCustomerSpec.Username,
                Password = testCustomerPassword + "WRONG"
            };

            var resultedEvent = await instanceUnderTest.Handle(loginCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(LoginFailedEvent));
                var specificResultedEvent = (LoginFailedEvent)resultedEvent;
                specificResultedEvent.Username.Should().Be(testCustomerSpec.Username);
            }
        }

        [Test]
        public async Task LoginFailed_WrongUsername()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var loginCommand = new LoginCommand
            {
                Username = testCustomerSpec.Username + "WRONG",
                Password = testCustomerPassword
            };

            var resultedEvent = await instanceUnderTest.Handle(loginCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(LoginFailedEvent));
                var specificResultedEvent = (LoginFailedEvent)resultedEvent;
                specificResultedEvent.Username.Should().Be(testCustomerSpec.Username + "WRONG");
            }
        }

        [Test]
        public async Task CustomerRegistrationSucceed()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(It.IsAny<string>())).
                ReturnsAsync(default(CustomerSpecification));
            dataBaseMock.Setup(d => d.SaveCustomer(It.IsAny<CustomerSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var registerCommand = new RegisterCustomerCommand
            {
                Customer = (CustomerSpecification)registerCustomerSpec.Clone()
            };

            var resultedEvent = await instanceUnderTest.Handle(registerCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CustomerRegisteredEvent));
                var specificResultedEvent = (CustomerRegisteredEvent)resultedEvent;
                specificResultedEvent.Customer.Username.Should().Be(registerCustomerSpec.Username);
                specificResultedEvent.Customer.FirstName.Should().Be(registerCustomerSpec.FirstName);
                specificResultedEvent.Customer.LastName.Should().Be(registerCustomerSpec.LastName);
                specificResultedEvent.Customer.Phonenumber.Should().Be(registerCustomerSpec.Phonenumber);
                specificResultedEvent.Customer.Password.Should().BeNull();
                specificResultedEvent.Customer.PasswordHash.Should().Be(exampleservice.CustomerService.CustomerService.ComputePasswordHash(registerCustomerSpec.Password));
                specificResultedEvent.Customer.CustomerId.Should().NotBeEmpty();
            }
        }

        [Test]
        public async Task CustomerRegistrationFailed_UsernameAlreadyTaken()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            var registerCommand = new RegisterCustomerCommand
            {
                Customer = registerCustomerSpec
            };

            var resultedEvent = await instanceUnderTest.Handle(registerCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CustomerRegistrationFailedEvent));
                var specificResultedEvent = (CustomerRegistrationFailedEvent)resultedEvent;
                specificResultedEvent.Customer.Should().Be(registerCustomerSpec);
            }
        }

        [Test]
        public async Task CheckSessionSucceed()
        {
            var testSessionId = Guid.NewGuid();
            var testSession = new SessionSpecification(){
                SessionId = testSessionId,
                CreatedAt = DateTime.Now.Subtract(new TimeSpan(0,5,0)),
                ValidNotAfter = DateTime.Now.Add(new TimeSpan(0,25,0))
            };

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
                specificResultedEvent.Session.ValidNotAfter.Should().BeAfter(testSession.ValidNotAfter); //TODO: better way to check renewal?
            }
        }

        [Test]
        public async Task CheckSessionFailed_SessionTimeout()
        {
            var testSessionId = Guid.NewGuid();
            var testSession = new SessionSpecification(){
                SessionId = testSessionId,
                CreatedAt = DateTime.Now.Subtract(new TimeSpan(0,35,0)),
                ValidNotAfter = DateTime.Now.Subtract(new TimeSpan(0,5,0))
            };

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
                resultedEvent.Should().BeOfType(typeof(SessionTimeoutEvent));
                var specificResultedEvent = (SessionTimeoutEvent)resultedEvent;
                specificResultedEvent.SessionId.Should().Be(testSession.SessionId);
            }
        }
    }
}
