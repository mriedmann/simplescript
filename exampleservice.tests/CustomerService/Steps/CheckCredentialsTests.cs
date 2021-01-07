using exampleservice.CustomerService;
using exampleservice.CustomerService.Contract;
using exampleservice.CustomerService.Controller;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace exampleservice.tests.CustomerService.Steps
{
    [TestFixture]
    public class CheckCredentialsTests
    {
        private const string testCustomerUsername = "testuser1";
        private const string testCustomerPassword = "password1234";

        private exampleservice.CustomerService.Steps.CheckCredentials instanceUnderTest;

        [SetUp]
        public void Setup()
        {
            var testCustomerSpec = new CustomerSpecification()
            {
                CustomerId = Guid.NewGuid(),
                Username = testCustomerUsername,
                Password = null,
                PasswordHash = exampleservice.CustomerService.Utils.Password.ComputeHash(testCustomerPassword)
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.LoadCustomer(testCustomerUsername))
                .ReturnsAsync(testCustomerSpec);

            instanceUnderTest = new exampleservice.CustomerService.Steps.CheckCredentials(dataBaseRepositoryMock.Object);
        }

        [Test]
        public async Task Execute_ValidUsernameAndValidPassword()
        {
            var context = new SessionContext { Command = new LoginCommand { Username = testCustomerUsername, Password = testCustomerPassword } };

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
                context.PasswordValid.Should().BeTrue();
            }
        }

        [Test]
        public async Task Execute_ValidUsernameAndInvalidPassword()
        {
            var context = new SessionContext { Command = new LoginCommand { Username = testCustomerUsername, Password = testCustomerPassword + "WRONG" } };

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
                context.PasswordValid.Should().BeFalse();
            }
        }

        [Test]
        public async Task Execute_InvalidUsernameAndInvalidPassword()
        {
            var context = new SessionContext { Command = new LoginCommand { Username = testCustomerUsername + "WRONG", Password = testCustomerPassword + "WRONG" } };

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
                context.PasswordValid.Should().BeFalse();
            }
        }

        [Test]
        public async Task Execute_InvalidUsernameAndValidPassword()
        {
            var context = new SessionContext { Command = new LoginCommand { Username = testCustomerUsername + "WRONG", Password = testCustomerPassword } };

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
                context.PasswordValid.Should().BeFalse();
            }
        }
    }
}