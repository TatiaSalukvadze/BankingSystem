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
using NuGet.Frameworks;

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

            _cardService = new CardService(_mockUnitOfWork.Object);
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

            var response = await _cardService.AuthorizeCardAsync(cardNumber, PIN);

            Assert.True(response.Success);
            Assert.Equal("Card validated", response.Message);
            Assert.Equal(card, response.Data);
            Assert.Equal(200, response.StatusCode);

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

            var response = await _cardService.AuthorizeCardAsync(cardNumber, PIN);

            Assert.False(response.Success);
            Assert.Equal("Card is expired!", response.Message);
            Assert.Equal(400, response.StatusCode);
            Assert.Null(response.Data);

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

            var response = await _cardService.CreateCardAsync(createCardDto);

            Assert.True(response.Success);
            Assert.Equal("Card was created successfully!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(201, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN), Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
            //_mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once());

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

            var response = await _cardService.CreateCardAsync(createCardDto);

            Assert.False(response.Success);
            Assert.Equal("Card number already exists!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(409, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN), Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
            //_mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never());

        }

        [Fact]
        public async Task SeeCardsAsync_ShouldSeeCards()
        {
            string email = "t@gmail.com";
            var cards = new List<CardWithIBANDTO> { new CardWithIBANDTO { } };
            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardsForPersonAsync(email)).ReturnsAsync(cards);

            var response = await _cardService.SeeCardsAsync(email);

            Assert.True(response.Success);
            Assert.Equal("Cards For Account (IBAN) were found!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());

        }

        [Fact]
        public async Task SeeCardsAsync_ShouldNotSeeCardsForNonexistendAccount()
        {
            string email = "t@gmail.com";
            var cards = new List<CardWithIBANDTO> { };
            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardsForPersonAsync(email)).ReturnsAsync(cards);

            var response = await _cardService.SeeCardsAsync(email);

            Assert.False(response.Success);
            Assert.Equal("You don't have accounts!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Never());

        }
        [Fact]
        public async Task CancelCardAsync_ShouldCancelCard()
        {
            string cardNumber = "4998892941729115";
          
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(cardNumber)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.DeleteCardAsync(cardNumber)).ReturnsAsync(true);

            var response = await _cardService.CancelCardAsync(cardNumber);

            Assert.True(response.Success);
            Assert.Equal("Card was successfully canceled!", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
            //_mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once());
        }

        [Fact]
        public async Task CancelCardAsync_ShouldNotCancelNonexistentCard()
        {
            string cardNumber = "4998892941729115";

            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(cardNumber)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CardRepository.DeleteCardAsync(cardNumber)).ReturnsAsync(false);

            var response = await _cardService.CancelCardAsync(cardNumber);

            Assert.False(response.Success);
            Assert.Equal("There is no Card for that Card Number!", response.Message);
            Assert.Equal(404, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
            //_mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never());
        }
        [Fact]
        public async Task SeeBalanceAsync_ShouldSeeBalance()
        {
            var cardAuthorizationDTO = new CardAuthorizationDTO { CardNumber = "4998892941729115", PIN = "1234" };

            Card card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = cardAuthorizationDTO.CardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = cardAuthorizationDTO.PIN
            };

            var seeBalanceDto = new SeeBalanceDTO { Amount = 100, Currency = CurrencyType.GEL };

            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(cardAuthorizationDTO.CardNumber)).ReturnsAsync(card);       

            _mockUnitOfWork.Setup(u => u.CardRepository.GetBalanceAsync(cardAuthorizationDTO)).ReturnsAsync(seeBalanceDto);

            var response = await _cardService.SeeBalanceAsync(cardAuthorizationDTO);

            Assert.True(response.Success);
            Assert.Equal("Balance retrieved successfully.", response.Message);
            Assert.Equal(seeBalanceDto, response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));

        }

        [Fact]
        public async Task SeeBalanceAsync_ShouldNotSeeBalance()
        {
            var cardAuthorizationDTO = new CardAuthorizationDTO { CardNumber = "4998892941729115", PIN = "1234" };

            Card card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = cardAuthorizationDTO.CardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = cardAuthorizationDTO.PIN
            };

            var seeBalanceDto = new SeeBalanceDTO { Amount = 0, Currency = 0};

            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(cardAuthorizationDTO.CardNumber)).ReturnsAsync(card);

            _mockUnitOfWork.Setup(u => u.CardRepository.GetBalanceAsync(cardAuthorizationDTO)).ReturnsAsync(seeBalanceDto);

            var response = await _cardService.SeeBalanceAsync(cardAuthorizationDTO);

            Assert.False(response.Success);
            Assert.Equal("Unable to retrieve balance.", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));

        }

        [Fact]
        public async Task ChangeCardPINAsync_ShouldChangePIN()
        {
            var changePINDto = new ChangeCardPINDTO { CardNumber = "4998892941729115", PIN = "1234", NewPIN = "4321" };
            Card card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = changePINDto.CardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = changePINDto.PIN
            };

            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(changePINDto.CardNumber)).ReturnsAsync(card);

            _mockUnitOfWork.Setup(u => u.CardRepository.UpdateCardAsync(card.Id, changePINDto.NewPIN)).ReturnsAsync(true);

            var response = await _cardService.ChangeCardPINAsync(changePINDto);

            Assert.True(response.Success);
            Assert.Equal($"Card PIN was updated Successfully! New PIN: {changePINDto.NewPIN}", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));

        }

        [Fact]
        public async Task ChangeCardPINAsync_ShouldNotChangePIN()
        {
            var changePINDto = new ChangeCardPINDTO { CardNumber = "4998892941729115", PIN = "1234", NewPIN = "4321" };
            Card card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = changePINDto.CardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = changePINDto.PIN
            };

            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(changePINDto.CardNumber)).ReturnsAsync(card);

            _mockUnitOfWork.Setup(u => u.CardRepository.UpdateCardAsync(card.Id, changePINDto.NewPIN)).ReturnsAsync(false);

            var response = await _cardService.ChangeCardPINAsync(changePINDto);

            Assert.False(response.Success);
            Assert.Equal("Card PIN could not be updated!", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));

        }
    }
}