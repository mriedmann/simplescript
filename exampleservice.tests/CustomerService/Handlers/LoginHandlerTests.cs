using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using exampleservice.Framework.Abstract;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace exampleservice.tests.CustomerService.Handlers
{
    [TestFixture]
    public class LoginHandlerTests
    {
        private const string testCustomerUsername = "testuser1";
        private const string testCustomerPassword = "password1234";
        private CustomerSpecification testCustomerSpec;
        private CustomerSpecification registerCustomerSpec;

        [SetUp]
        public void Setup()
        {
            testCustomerSpec = new CustomerSpecification()
            {
                CustomerId = Guid.NewGuid(),
                FirstName = "FN",
                LastName = "LN",
                Username = testCustomerUsername,
                Phonenumber = "+123456789",
                DayOfBirth = new DateTime(1999, 9, 9),
                Password = null,
                PasswordHash = exampleservice.CustomerService.Utils.Password.ComputeHash(testCustomerPassword)
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
                specificResultedEvent.Session.ValidNotAfter.Should().BeAfter(DateTime.Now.AddMinutes(25));
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
        public async Task LoginFailed_MissingArgument_Command()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);
            LoginCommand loginCommand = null;

            try
            {
                await instanceUnderTest.Handle(loginCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentExeption was expected but not thrown");
        }

        [Test]
        public async Task LoginFailed_MissingArgument_Username()
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
                Username = null,
                Password = testCustomerPassword
            };

            try
            {
                await instanceUnderTest.Handle(loginCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentExeption was expected but not thrown");
        }

        [Test]
        public async Task LoginFailed_MissingArgument_Password()
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
                Username = testCustomerUsername,
                Password = null
            };

            try
            {
                await instanceUnderTest.Handle(loginCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentExeption was expected but not thrown");
        }
    }
}
