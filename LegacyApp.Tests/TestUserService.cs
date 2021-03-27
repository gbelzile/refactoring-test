using Moq;
using System;
using Xunit;

namespace LegacyApp.Tests
{
    public sealed class TestUserService
    {
        private readonly UserService _userService;
        private int _creditLimit = 0;
        private string _clientName = "AnyClient";

        public TestUserService()
        {
            var clientRepository = new Mock<IClientRepository>();

             // only id 1 is valid, any other returns null
            clientRepository.Setup(x => x.GetById(It.IsIn(1)))
                .Returns((int x) => new Client {
                    ClientStatus = ClientStatus.none,
                    Id = x,
                    Name = _clientName
                });

            var userCreditService = new Mock<IUserCreditService>();
            userCreditService.Setup(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(() => _creditLimit);
            var userCreditServiceFactory = new Mock<UserCreditServiceFactory>();
            userCreditServiceFactory.Setup(x => x.GetCreditService()).Returns(userCreditService.Object);
            var userDataRepository = new Mock<IUserDataRepository>();
            _userService = new UserService(clientRepository.Object, userCreditServiceFactory.Object, userDataRepository.Object);
        }

        public static readonly object[][] TestInvalidParameterNameReturnsFalse_Data =
        {
            //null firstname
            new object[] { null, "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25)},
            //empty firstname
            new object[] { "", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25)},
            //null lastname
            new object[] { "Guilllaume", null, "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25)},
            //empty lastname
            new object[] { "Guilllaume", "", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25)},
            //null email
            new object[] { "Guilllaume", "Belzile", null, new DateTime(1979, 9, 25)},
            //empty email
            new object[] { "Guilllaume", "Belzile", "", new DateTime(1979, 9, 25)},
            //email without .
            new object[] { "Guilllaume", "Belzile", "guillaumebelzile@hotmailcom", new DateTime(1979, 9, 25)},
            //email without @.
            new object[] { "Guilllaume", "Belzile", "guillaumebelzilehotmail.com", new DateTime(1979, 9, 25)},
        };

        [Theory, MemberData(nameof(TestInvalidParameterNameReturnsFalse_Data))]
        public void TestInvalidParameterNameReturnsFalse(string firstName, string lastName, string email, DateTime birthDate)
        {
            Assert.False(_userService.AddUser(firstName, lastName, email, birthDate, 1));
        }

        [Fact]
        public void TestInvalidClientIdReturnsFalse()
        {
            Assert.False(_userService.AddUser("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25), 2));
        }

        [Fact]
        public void TestInvalidCreditReturnsFalse()
        {
            // CreditLimit must be > 500
            _creditLimit = 100;
            Assert.False(_userService.AddUser("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25), 1));
        }

        [Fact]
        public void TestInvalidCreditForImportantClientReturnsFalse()
        {
            // CreditLimit must be > 500
            // This client doubles the credit limit so it will be < 500
            _clientName = "ImportantClient";
            _creditLimit = 240;
            Assert.False(_userService.AddUser("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25), 1));
        }

        [Fact]
        public void TestInvalidAgeReturnsFalse()
        {
            _creditLimit = 1000;
            Assert.False(_userService.AddUser(new UserDto("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1999, 01, 02), () => new DateTime(2021, 01, 01)), 1));
        }

        [Fact]
        public void TestValidAgeReturnsTrue()
        {
            _creditLimit = 1000;
            Assert.True(_userService.AddUser(new UserDto("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1998, 12, 31), () => new DateTime(2021, 1, 1)), 1));
        }

        [Fact]
        public void TestNoCreditLimitReturnsTrue()
        {
            // This client doesn't have credit limit
            _clientName = "VeryImportantClient";
            _creditLimit = 100;
            Assert.True(_userService.AddUser("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25), 1));
        }

        [Fact]
        public void TestNoDoubleCreditLimitReturnsTrue()
        {
            // This client doubles the credit limit so it will be > 500
            _clientName = "ImportantClient";
            _creditLimit = 300;
            Assert.True(_userService.AddUser("Guillaume", "Belzile", "guillaumebelzile@hotmail.com", new DateTime(1979, 9, 25), 1));
        }
    }
}
