﻿using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BankingSystem.UnitTests
{
    public class TransactionDetailsServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IExchangeRateService> _mockExchangeRateService;
        private readonly TransactionDetailsService _transactionDetailsService;

        public TransactionDetailsServiceTests()
        {
            var cardRepositoryMock = new Mock<ICardRepository>();
            var transactionDetailsRepositoryMock = new Mock<ITransactionDetailsRepository>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExchangeRateService = new Mock<IExchangeRateService>();

            _mockUnitOfWork.Setup(u => u.CardRepository).Returns(cardRepositoryMock.Object);
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository).Returns(transactionDetailsRepositoryMock.Object);

            _transactionDetailsService = new TransactionDetailsService(_mockConfiguration.Object, _mockUnitOfWork.Object, _mockExchangeRateService.Object);
        }

    }
}
