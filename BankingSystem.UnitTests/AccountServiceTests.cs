using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.UnitTests
{
    public class AccountServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            var accountRepositoryMock = new Mock<IAccountRepository>();
            var personRepositoryMock = new Mock<IPersonRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.PersonRepository).Returns(personRepositoryMock.Object);
            _mockUnitOfWork.Setup(u => u.AccountRepository).Returns(accountRepositoryMock.Object);
            _accountService = new AccountService(_mockUnitOfWork.Object);
        }

      
    }
}
