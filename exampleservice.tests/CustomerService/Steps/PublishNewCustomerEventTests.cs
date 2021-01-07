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
    public class PublishNewCustomerEventTests
    {
        private const string testCustomerUsername = "testuser1";
        private const string testCustomerPassword = "password1234";
        private string testCustomerPasswordHash = exampleservice.CustomerService.Utils.Password.ComputeHash(testCustomerPassword);
        private Guid testCustomerId = new Guid("11111111-1111-1111-1111-111111111111");
        private CustomerSpecification testCommandCustomerSpec;
        private CustomerSpecification testSavedCustomerSpec;

        [SetUp]
        public void Setup()
        {
            testCommandCustomerSpec = new CustomerSpecification()
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

            testSavedCustomerSpec = new CustomerSpecification()
            {
                CustomerId = testCustomerId,
                FirstName = testCommandCustomerSpec.FirstName,
                LastName = testCommandCustomerSpec.LastName,
                Username = testCustomerUsername,
                Phonenumber = testCommandCustomerSpec.Phonenumber,
                DayOfBirth = testCommandCustomerSpec.DayOfBirth,
                Password = null,
                PasswordHash = testCustomerPasswordHash
            };
        }

        [Test]
        public async Task Execute_RepoSaveOk()
        {
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.PublishEvent(It.IsAny<CustomerRegisteredEvent>()));

            var instanceUnderTest = new exampleservice.CustomerService.Steps.PublishNewCustomerEvent(messageBusMock.Object);
            var context = new CustomerContext { Command = new RegisterCustomerCommand { Customer = testCommandCustomerSpec }, Customer = testSavedCustomerSpec };
            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
            }
            messageBusMock.Verify(m => m.PublishEvent(It.Is<CustomerRegisteredEvent>(e => e.Customer == testSavedCustomerSpec)));
        }

        [Test]
        public async Task Componsate()
        {
            var messageBusMock = new Moq.Mock<IMessageBus>();
            messageBusMock.Setup(s => s.PublishEvent(It.IsAny<CustomerRemovedEvent>()));

            var instanceUnderTest = new exampleservice.CustomerService.Steps.PublishNewCustomerEvent(messageBusMock.Object);
            var context = new CustomerContext { Command = new RegisterCustomerCommand { Customer = testCommandCustomerSpec }, Customer = testSavedCustomerSpec };
            await instanceUnderTest.Compensate(context);

            messageBusMock.Verify(m => m.PublishEvent(It.Is<CustomerRemovedEvent>(e => e.Customer == testSavedCustomerSpec)));
        }
    }
}