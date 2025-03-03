using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Contracts.DTOs.OnlineBank;
using System.Net.NetworkInformation;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Domain.Enums;
using BankingSystem.Contracts.Interfaces.IServices;

namespace BankingSystem.UnitTests
{
    public class CardServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CardService _cardService;

        public CardServiceTests()
        {
            var cardRepositoryMock = new Mock<ICardRepository>();
            var accountRepositoryMock = new Mock<IAccountRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.CardRepository).Returns(cardRepositoryMock.Object);
            _mockUnitOfWork.Setup(u => u.AccountRepository).Returns(accountRepositoryMock.Object);

            var configurationMock = new Mock<IConfiguration>();
            var exchangeRateMock = new Mock<IExchangeRateService>();
            _cardService = new CardService(configurationMock.Object, _mockUnitOfWork.Object, exchangeRateMock.Object);
        }

        [Fact]
        public async Task AuthorizeCardAsync_ShouldAuthorizeCard()
        {
            string cardNumber = "4998892941729115";
            string PIN = "1234";
            Card card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = cardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = PIN
            };
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(cardNumber)).ReturnsAsync(card);

            var (success, message, data) = await _cardService.AuthorizeCardAsync(cardNumber, PIN);

            Assert.True(success);
            Assert.Equal("Card validated", message);
            Assert.Equal(card, data);

            _mockUnitOfWork.Verify(u => u.CardRepository.GetCardAsync(cardNumber), Times.Once());

        }
        [Fact]
        public async Task AuthorizeCardAsync_ShouldNotAuthorizeExpiredCard()
        {
            string cardNumber = "4998892941729115";
            string PIN = "1234";
            var card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = cardNumber,
                ExpirationDate = "09/24",
                CVV = "123",
                PIN = PIN
            };
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(cardNumber)).ReturnsAsync(card);

            var (success, message, data) = await _cardService.AuthorizeCardAsync(cardNumber, PIN);

            Assert.False(success);
            Assert.Equal("Card is expired!", message);
            Assert.Null(data);

            _mockUnitOfWork.Verify(u => u.CardRepository.GetCardAsync(cardNumber), Times.Once());

        }
        [Fact]
        public async Task CreateCardAsync_SHouldCreateCard()
        {
            var createCardDto = new CreateCardDTO
            {
                IBAN = "GE38CD9670132279404959",
                CardNumber = "4998892941729115",
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = "4321"
            };
            var account = new Account { Id = 1 };
            _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN)).ReturnsAsync(account);
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(createCardDto.CardNumber)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CardRepository.CreateCardAsync(It.IsAny<Card>())).ReturnsAsync(1);

            var (success, message, data) = await _cardService.CreateCardAsync(createCardDto);

            Assert.True(success);
            Assert.Equal("Card was created successfully!", message);
            Assert.NotNull(data);

            _mockUnitOfWork.Verify(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN), Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once());

        }
        [Fact]
        public async Task CreateCardAsync_SHouldNotCreateExistingCard()
        {
            var createCardDto = new CreateCardDTO
            {
                IBAN = "GE38CD9670132279404959",
                CardNumber = "4998892941729115",
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = "4321"
            };
            var account = new Account { Id = 1 };
            _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN)).ReturnsAsync(account);
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(createCardDto.CardNumber)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.CreateCardAsync(It.IsAny<Card>())).ReturnsAsync(0);

            var (success, message, data) = await _cardService.CreateCardAsync(createCardDto);

            Assert.False(success);
            Assert.Equal("Card number already exists!", message);
            Assert.Null(data);

            _mockUnitOfWork.Verify(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN), Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never());

        }

        [Fact]
        public async Task SeeCardsAsync_ShouldSeeCards()
        {
            string email = "t@gmail.com";
            var cards = new List<CardWithIBANDTO> { new CardWithIBANDTO { } };
            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardsForPersonAsync(email)).ReturnsAsync(cards);

            var (success, message, data) = await _cardService.SeeCardsAsync(email);

            Assert.True(success);
            Assert.Equal("Cards For Account (IBAN) were found!", message);
            Assert.NotNull(data);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());

        }

        [Fact]
        public async Task SeeCardsAsync_ShouldNotSeeCardsForNonexistendAccount()
        {
            string email = "t@gmail.com";
            var cards = new List<CardWithIBANDTO> {  };
            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardsForPersonAsync(email)).ReturnsAsync(cards);

            var (success, message, data) = await _cardService.SeeCardsAsync(email);

            Assert.False(success);
            Assert.Equal("You don't have accounts!", message);
            Assert.Null(data);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Never());

        }
        [Fact]
        public async Task CancelCardAsync_ShouldCancelCard()
        {
            string cardNumber = "4998892941729115";
          
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(cardNumber)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.DeleteCardAsync(cardNumber)).ReturnsAsync(true);

            var (success, message) = await _cardService.CancelCardAsync(cardNumber);

            Assert.True(success);
            Assert.Equal("Card was successfully canceled!", message);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once());
        }

        [Fact]
        public async Task CancelCardAsync_ShouldNotCancelNonexistentCard()
        {
            string cardNumber = "4998892941729115";

            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(cardNumber)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CardRepository.DeleteCardAsync(cardNumber)).ReturnsAsync(false);

            var (success, message) = await _cardService.CancelCardAsync(cardNumber);

            Assert.False(success);
            Assert.Equal("There is no Card for that Card Number!", message);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never());
        }
        //[Fact]
        //public async Task SeeBalanceAsync_ShouldSeeBalance()
        //{
        //    string cardNumber = "4998892941729115";
        //    string PIN = "1234";

        //    var seeBalanceDto = new SeeBalanceDTO { Amount = 100, Currency = CurrencyType.GEL};
        //    var _cardServiceMock = new Mock<ICardService>();// (_cardService)
        //    //{
        //    //    CallBase = true // This allows partial mocking
        //    //};

        //    _cardServiceMock.Setup(s => s.AuthorizeCardAsync(cardNumber, PIN)).ReturnsAsync((true,"",null));
        //    _cardServiceMock.Setup(s => s.SeeBalanceAsync(cardNumber, PIN))
        //        .ReturnsAsync( await _cardService.SeeBalanceAsync(cardNumber, PIN));

        //    //_mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(cardNumber)).ReturnsAsync(new Card { });
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.GetBalanceAsync(cardNumber, PIN)).ReturnsAsync(seeBalanceDto);

        //    var (success, message, data) = await _cardServiceMock.Object.SeeBalanceAsync(cardNumber, PIN);

        //    Assert.True(success);
        //    Assert.Equal("Balance retrieved successfully.", message);
        //    Assert.Equal(seeBalanceDto,data);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once());

        //}
    }
}
