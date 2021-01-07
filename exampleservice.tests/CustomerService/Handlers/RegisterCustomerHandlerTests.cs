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
    public class RegisterCustomerHandlerTests
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
        public async Task CustomerRegistrationSucceed()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(It.IsAny<string>())).
                ReturnsAsync(default(CustomerSpecification));
            dataBaseMock.Setup(d => d.CreateCustomer(It.IsAny<CustomerSpecification>())).
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
                specificResultedEvent.Customer.PasswordHash.Should().Be(exampleservice.CustomerService.Utils.Password.ComputeHash(registerCustomerSpec.Password));
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
        public async Task CustomerRegistrationFailed_ArgumentMissing_Username()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);

            registerCustomerSpec.Username = null;
            var registerCommand = new RegisterCustomerCommand
            {
                Customer = registerCustomerSpec
            };

            try
            {
                await instanceUnderTest.Handle(registerCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentExeption was expected but not thrown");
        }

        [Test]
        public async Task CustomerRegistrationFailed_ArgumentMissing_Password()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);

            registerCustomerSpec.Password = null;
            var registerCommand = new RegisterCustomerCommand
            {
                Customer = registerCustomerSpec
            };

            try
            {
                await instanceUnderTest.Handle(registerCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentExeption was expected but not thrown");
        }

        [Test]
        public async Task CustomerRegistrationFailed_ArgumentMissing_CustomerSpec()
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
                Customer = null
            };

            try
            {
                await instanceUnderTest.Handle(registerCommand);
            }
            catch (ArgumentException)
            {
                Assert.Pass("Caught expected ArgumentException");
            }
            Assert.Fail("ArgumentExeption was expected but not thrown");
        }

        [Test]
        public async Task CustomerRegistrationFailed_OffensiveArgument_PasswordHash()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);

            registerCustomerSpec.PasswordHash = "somehash";
            var registerCommand = new RegisterCustomerCommand
            {
                Customer = registerCustomerSpec
            };

            try
            {
                await instanceUnderTest.Handle(registerCommand);
            }
            catch (NotSupportedException)
            {
                Assert.Pass("Caught expected NotSupportedException");
            }
            Assert.Fail("NotSupportedException was expected but not thrown");
        }

        [Test]
        public async Task CustomerRegistrationFailed_OffensiveArgument_CustomerId()
        {
            var busMock = new Moq.Mock<IMessageBus>();

            var dataBaseMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.LoadCustomer(testCustomerSpec.Username)).
                ReturnsAsync(testCustomerSpec);
            dataBaseMock.Setup(d => d.SaveSession(It.IsAny<SessionSpecification>())).
                ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.CustomerService(busMock.Object, dataBaseMock.Object);

            registerCustomerSpec.CustomerId = Guid.NewGuid();
            var registerCommand = new RegisterCustomerCommand
            {
                Customer = registerCustomerSpec
            };

            try
            {
                await instanceUnderTest.Handle(registerCommand);
            }
            catch (NotSupportedException)
            {
                Assert.Pass("Caught expected NotSupportedException");
            }
            Assert.Fail("NotSupportedException was expected but not thrown");
        }
    }
}
