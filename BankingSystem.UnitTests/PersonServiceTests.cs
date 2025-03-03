using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace BankingSystem.UnitTests
{
    public class PersonServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly PersonService _personService;

        public PersonServiceTests()
        {
            var personRepositoryMock = new Mock<IPersonRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.PersonRepository).Returns(personRepositoryMock.Object);
            _personService = new PersonService(_mockUnitOfWork.Object);
        }
        [Fact]
        public async Task RegisterCustomPersonAsync_ShouldRegisterPerson()
        {
            var registerDto = new RegisterPersonDTO { Name = "Tatia", Surname = "Salu", IDNumber = "33001059400",
                Birthdate = DateTime.Now, Email = "t@gmail.com", Password = "TatiaSalu0*", ClientUrl = "" };
            string identityUserId = Guid.NewGuid().ToString();

            _mockUnitOfWork.Setup(u => u.PersonRepository.RegisterPersonAsync(It.IsAny<Person>())).ReturnsAsync(1);

            var (success, message, data) = await _personService.RegisterCustomPersonAsync(registerDto, identityUserId);

            Assert.True(success);
            Assert.Equal("User was registered successfully!", message);
            Assert.NotNull(data);

            _mockUnitOfWork.Verify(u => u.PersonRepository.RegisterPersonAsync(It.IsAny<Person>()), Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once());
        }

        [Fact]
        public async Task RegisterCustomPersonAsync_ShouldNotRegisterPerson()
        {
            var registerDto = new RegisterPersonDTO { Name = "Tamar", Surname = "Salu", IDNumber = "33001059400",//same IDNUmber
                Birthdate = DateTime.Now, Email = "t@gmail.com", Password = "TamarSalu0*", ClientUrl = "" };
            string identityUserId = Guid.NewGuid().ToString();

            _mockUnitOfWork.Setup(u => u.PersonRepository.RegisterPersonAsync(It.IsAny<Person>())).ReturnsAsync(0);
            var (success, message, data) = await _personService.RegisterCustomPersonAsync(registerDto, identityUserId);

            Assert.False(success);
            Assert.Equal("Adding user in physical person system failed!", message);
            Assert.Null(data);

            _mockUnitOfWork.Verify(u => u.PersonRepository.RegisterPersonAsync(It.IsAny<Person>()), Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never());
        }

        [Fact]
        public async Task RegisteredPeopleStatisticsAsync_ShouldReturnValues()
        {
            _mockUnitOfWork.Setup(u => u.PersonRepository.PeopleRegisteredThisYear()).ReturnsAsync(10);
            _mockUnitOfWork.Setup(u => u.PersonRepository.PeopleRegisteredLastOneYear()).ReturnsAsync(10);
            _mockUnitOfWork.Setup(u => u.PersonRepository.PeopleRegisteredLast30Days()).ReturnsAsync(10);

            var (success, message, data) = await _personService.RegisteredPeopleStatisticsAsync();

            Assert.True(success);
            Assert.Equal("Statistics are retrieved!", message);
            Assert.NotNull(data);
            Assert.Equal(3, data.Count);
            _mockUnitOfWork.Verify((u => u.PersonRepository), Times.Exactly(3));

        }

    }
}