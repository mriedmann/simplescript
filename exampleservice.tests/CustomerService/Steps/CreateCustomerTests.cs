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
    public class CreateCustomerTests
    {
        private const string testCustomerUsername = "testuser1";
        private const string testCustomerPassword = "password1234";
        private string testCustomerPasswordHash = exampleservice.CustomerService.Utils.Password.ComputeHash(testCustomerPassword);
        private Guid testCustomerId = new Guid("11111111-1111-1111-1111-111111111111");

        [Test]
        public async Task Execute_RepoSaveOk()
        {
            var testCustomerSpec = new CustomerSpecification()
            {
                CustomerId = Guid.Empty,
                FirstName = "FN",
                LastName = "LN",
                Username = testCustomerUsername,
                Phonenumber = "+123456789",
                DayOfBirth = new DateTime(1999, 9, 9),
                Password = testCustomerPassword,
                PasswordHash = null
            };

            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.CreateCustomer(It.IsAny<CustomerSpecification>()))
                .ReturnsAsync(1);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.CreateCustomer(dataBaseRepositoryMock.Object);
            var context = new CustomerContext { Command = new RegisterCustomerCommand { Customer = testCustomerSpec } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
                context.Customer.CustomerId.Should().NotBe(Guid.Empty);
                context.Customer.FirstName.Should().Be(testCustomerSpec.FirstName);
                context.Customer.LastName.Should().Be(testCustomerSpec.LastName);
                context.Customer.Username.Should().Be(testCustomerSpec.Username);
                context.Customer.Phonenumber.Should().Be(testCustomerSpec.Phonenumber);
                context.Customer.DayOfBirth.Should().Be(testCustomerSpec.DayOfBirth);
                context.Customer.PasswordHash.Should().Be(testCustomerPasswordHash);
                context.Customer.Password.Should().Be(null);
            }
        }

        [Test]
        public async Task Execute_RepoSaveFailed()
        {
            var testCustomerSpec = new CustomerSpecification()
            {
                CustomerId = Guid.Empty,
                FirstName = "FN",
                LastName = "LN",
                Username = testCustomerUsername,
                Phonenumber = "+123456789",
                DayOfBirth = new DateTime(1999, 9, 9),
                Password = testCustomerPassword,
                PasswordHash = null
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.CreateCustomer(It.IsAny<CustomerSpecification>()))
                .ReturnsAsync(0);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.CreateCustomer(dataBaseRepositoryMock.Object);
            var context = new CustomerContext { Command = new RegisterCustomerCommand { Customer = testCustomerSpec } };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
            }
        }

        [Test]
        public async Task Componsate()
        {
            var testCustomerSpec = new CustomerSpecification()
            {
                CustomerId = testCustomerId,
                FirstName = "FN",
                LastName = "LN",
                Username = testCustomerUsername,
                Phonenumber = "+123456789",
                DayOfBirth = new DateTime(1999, 9, 9),
                Password = null,
                PasswordHash = testCustomerPasswordHash
            };
            var dataBaseRepositoryMock = new Moq.Mock<ICustomerServiceDataBaseRepository>();
            dataBaseRepositoryMock
                .Setup(d => d.DeleteCustomer(It.IsAny<Guid>()))
                .ReturnsAsync(0);

            var instanceUnderTest = new exampleservice.CustomerService.Steps.CreateCustomer(dataBaseRepositoryMock.Object);
            var context = new CustomerContext { Customer = testCustomerSpec };
            await instanceUnderTest.Compensate(context);

            dataBaseRepositoryMock.Verify(d => d.DeleteCustomer(testCustomerId));
        }
    }
}