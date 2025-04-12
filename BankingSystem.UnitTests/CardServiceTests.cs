using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using BankingSystem.Domain.Entities;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Domain.Enums;
using BankingSystem.Contracts.Interfaces.IServices;

namespace BankingSystem.UnitTests
{
    public class CardServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEncryptionService> _mockEncryptionService;
        private readonly Mock<IHashingService> _mockHashingService;
        private readonly CardService _cardService;

        public CardServiceTests()
        {
            var cardRepositoryMock = new Mock<ICardRepository>();
            var accountRepositoryMock = new Mock<IAccountRepository>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEncryptionService = new Mock<IEncryptionService>();
            _mockHashingService = new Mock<IHashingService>();

            _mockUnitOfWork.Setup(u => u.CardRepository).Returns(cardRepositoryMock.Object);
            _mockUnitOfWork.Setup(u => u.AccountRepository).Returns(accountRepositoryMock.Object);

            _cardService = new CardService(_mockUnitOfWork.Object,_mockEncryptionService.Object, _mockHashingService.Object);
        }

        [Fact]
        public async Task AuthorizeCardAsync_ShouldAuthorizeCard()
        {
            string cardNumber = "4998892941729115";
            string PIN = "1234";
            string encryptedCardNumber = $"encrypted{cardNumber}";
            string hashedPin = $"hashed{PIN}";
            Card card = new()
            {
                Id = 1,
                AccountId = 1,
                CardNumber = encryptedCardNumber,
                ExpirationDate = "09/27",
                CVV = "encrypted123",
                PIN = hashedPin
            };
            _mockEncryptionService.Setup(e => e.Encrypt(cardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(encryptedCardNumber)).ReturnsAsync(card);
            _mockHashingService.Setup(h => h.VerifyValue(PIN, card.PIN)).Returns(true);

            var response = await _cardService.AuthorizeCardAsync(cardNumber, PIN);

            Assert.True(response.Success);
            Assert.Equal("Card validated!", response.Message);
            Assert.Equal(card, response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository.GetCardAsync(encryptedCardNumber), Times.Once());
        }

        [Fact]
        public async Task AuthorizeCardAsync_ShouldNotAuthorizeExpiredCard()
        {
            string cardNumber = "4998892941729115";
            string PIN = "1234";
            string encryptedCardNumber = $"encrypted{cardNumber}";
            string hashedPin = $"hashed{PIN}";
            var card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = encryptedCardNumber,
                ExpirationDate = "04/24",
                CVV = "encrypted123",
                PIN = hashedPin
            };
            _mockEncryptionService.Setup(e => e.Encrypt(cardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(encryptedCardNumber)).ReturnsAsync(card);
            _mockHashingService.Setup(h => h.VerifyValue(PIN, card.PIN)).Returns(true);

            var response = await _cardService.AuthorizeCardAsync(cardNumber, PIN);

            Assert.False(response.Success);
            Assert.Equal("Card is expired!", response.Message);
            Assert.Equal(400, response.StatusCode);
            Assert.Null(response.Data);

            _mockUnitOfWork.Verify(u => u.CardRepository.GetCardAsync(encryptedCardNumber), Times.Once());
        }

        [Fact]
        public async Task CreateCardAsync_ShouldCreateCard()
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
            var card = new Card
            {
                AccountId = account.Id,
                CardNumber = $"encrypted{createCardDto.CardNumber}",
                ExpirationDate = "09/27",
                CVV = $"encrypted{createCardDto.CVV}",
                PIN = $"hashed{createCardDto.PIN}"
            };

            _mockEncryptionService.Setup(e => e.Encrypt(createCardDto.CardNumber)).Returns(card.CardNumber);
            _mockEncryptionService.Setup(e => e.Encrypt(createCardDto.CVV)).Returns(card.CVV);
            _mockHashingService.Setup(h => h.HashValue(createCardDto.PIN)).Returns(card.PIN);
            _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN)).ReturnsAsync(account);
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(card.CardNumber)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CardRepository.CreateCardAsync(It.IsAny<Card>())).ReturnsAsync(1);

            var response = await _cardService.CreateCardAsync(createCardDto);

            Assert.True(response.Success);
            Assert.Equal("Card was created successfully!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(201, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN), Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task CreateCardAsync_ShouldNotCreateExistingCard()
        {
            var createCardDto = new CreateCardDTO
            {
                IBAN = "GE38CD9670132279404959",
                CardNumber = "4998892941729115",
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = "4321"
            };
            string encryptedCardNumber = $"encrypted{createCardDto.CardNumber}";
            var account = new Account { Id = 1 };
            _mockEncryptionService.Setup(e => e.Encrypt(createCardDto.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN)).ReturnsAsync(account);
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(encryptedCardNumber)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.CreateCardAsync(It.IsAny<Card>())).ReturnsAsync(0);

            var response = await _cardService.CreateCardAsync(createCardDto);

            Assert.False(response.Success);
            Assert.Equal("Card number already exists!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(409, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN), Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
        }

        [Fact]
        public async Task SeeCardsAsync_ShouldSeeCards()
        {
            string email = "t@gmail.com";
            int page = 1; 
            int perPage = 2;
            var paginatedCards = new List<CardWithIBANDTO> { new CardWithIBANDTO { }, new CardWithIBANDTO { } };
            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmailAsync(email)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardsCountForPersonAsync(email)).ReturnsAsync(3);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardsForPersonAsync(email, It.IsAny<int>(),perPage)).ReturnsAsync(paginatedCards);

            var response = await _cardService.SeeCardsAsync(email, page, perPage);

            Assert.True(response.Success);
            Assert.Equal("Cards For Account (IBAN) were found!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.Data.TotalDataCount);
            Assert.Equal(2, response.Data.Data.Count);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once());
            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task SeeCardsAsync_ShouldNotSeeCardsForNonexistentAccount()
        {
            string email = "t@gmail.com";
            int page = 1;
            int perPage = 2;
            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmailAsync(email)).ReturnsAsync(false);

            var response = await _cardService.SeeCardsAsync(email, page, perPage);

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
            var deleteCardDto = new DeleteCardDTO { CardNumber = "4998892941729115" };
            string encryptedCardNumber = $"encrypted{deleteCardDto.CardNumber}";

            _mockEncryptionService.Setup(e => e.Encrypt(deleteCardDto.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(encryptedCardNumber)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.DeleteCardAsync(encryptedCardNumber)).ReturnsAsync(true);

            var response = await _cardService.CancelCardAsync(deleteCardDto);

            Assert.True(response.Success);
            Assert.Equal("Card was successfully canceled!", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task CancelCardAsync_ShouldNotCancelNonexistentCard()
        {
            var deleteCardDto = new DeleteCardDTO { CardNumber = "4998892941729115" };
            string encryptedCardNumber = $"encrypted{deleteCardDto.CardNumber}";

            _mockEncryptionService.Setup(e => e.Encrypt(deleteCardDto.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.CardNumberExistsAsync(encryptedCardNumber)).ReturnsAsync(false);

            var response = await _cardService.CancelCardAsync(deleteCardDto);

            Assert.False(response.Success);
            Assert.Equal("There is no Card for that Card Number!", response.Message);
            Assert.Equal(404, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
        }

        [Fact]
        public async Task SeeBalanceAsync_ShouldSeeBalance()
        {
            var cardAuthorizationDTO = new CardAuthorizationDTO { CardNumber = "4998892941729115", PIN = "1234" };
            string encryptedCardNumber = $"encrypted{cardAuthorizationDTO.CardNumber}";
            var card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = encryptedCardNumber,
                ExpirationDate = "09/27",
                CVV = "encrypted123",
                PIN = $"hashed{cardAuthorizationDTO.PIN}"
            };

            var seeBalanceDto = new SeeBalanceDTO { Amount = 100, Currency = CurrencyType.GEL };

            _mockEncryptionService.Setup(e => e.Encrypt(cardAuthorizationDTO.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(encryptedCardNumber)).ReturnsAsync(card);
            _mockHashingService.Setup(h => h.VerifyValue(cardAuthorizationDTO.PIN, card.PIN)).Returns(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetBalanceAsync(card.CardNumber, card.PIN)).ReturnsAsync(seeBalanceDto);

            var response = await _cardService.SeeBalanceAsync(cardAuthorizationDTO);

            Assert.True(response.Success);
            Assert.Equal("Balance retrieved successfully!", response.Message);
            Assert.Equal(seeBalanceDto, response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task SeeBalanceAsync_ShouldNotSeeBalance()
        {
            var cardAuthorizationDTO = new CardAuthorizationDTO { CardNumber = "4998892941729115", PIN = "1234" };
            string encryptedCardNumber = $"encrypted{cardAuthorizationDTO.CardNumber}";
            var card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = encryptedCardNumber,
                ExpirationDate = "09/27",
                CVV = "encrypted123",
                PIN = $"hashed{cardAuthorizationDTO.PIN}"
            };

            var seeBalanceDto = new SeeBalanceDTO { Amount = 0, Currency = 0 };

            _mockEncryptionService.Setup(e => e.Encrypt(cardAuthorizationDTO.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(encryptedCardNumber)).ReturnsAsync(card);
            _mockHashingService.Setup(h => h.VerifyValue(cardAuthorizationDTO.PIN, card.PIN)).Returns(true);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetBalanceAsync(card.CardNumber, card.PIN)).ReturnsAsync(seeBalanceDto);

            var response = await _cardService.SeeBalanceAsync(cardAuthorizationDTO);

            Assert.False(response.Success);
            Assert.Equal("Unable to retrieve balance!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task ChangeCardPINAsync_ShouldChangePIN()
        {
            var changePINDto = new ChangeCardPINDTO { CardNumber = "4998892941729115", PIN = "1234", NewPIN = "4321" };
            string encryptedCardNumber = $"encrypted{changePINDto.CardNumber}";
            var card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = encryptedCardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = changePINDto.PIN
            };
            string hashedNewPin = $"hashed{changePINDto.NewPIN}";
            _mockEncryptionService.Setup(e => e.Encrypt(changePINDto.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(encryptedCardNumber)).ReturnsAsync(card);
            _mockHashingService.Setup(h => h.VerifyValue(changePINDto.PIN, card.PIN)).Returns(true);
            _mockHashingService.Setup(h => h.HashValue(changePINDto.NewPIN)).Returns(hashedNewPin);
            _mockUnitOfWork.Setup(u => u.CardRepository.UpdateCardAsync(card.Id, hashedNewPin)).ReturnsAsync(true);

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
            string encryptedCardNumber = $"encrypted{changePINDto.CardNumber}";
            var card = new Card
            {
                Id = 1,
                AccountId = 1,
                CardNumber = encryptedCardNumber,
                ExpirationDate = "09/27",
                CVV = "123",
                PIN = changePINDto.PIN
            };
            string hashedNewPin = $"hashed{changePINDto.NewPIN}";
            _mockEncryptionService.Setup(e => e.Encrypt(changePINDto.CardNumber)).Returns(encryptedCardNumber);
            _mockUnitOfWork.Setup(u => u.CardRepository.GetCardAsync(encryptedCardNumber)).ReturnsAsync(card);
            _mockHashingService.Setup(h => h.VerifyValue(changePINDto.PIN, card.PIN)).Returns(true);
            _mockHashingService.Setup(h => h.HashValue(changePINDto.NewPIN)).Returns(hashedNewPin);
            _mockUnitOfWork.Setup(u => u.CardRepository.UpdateCardAsync(card.Id, hashedNewPin)).ReturnsAsync(false);

            var response = await _cardService.ChangeCardPINAsync(changePINDto);

            Assert.False(response.Success);
            Assert.Equal("Card PIN could not be updated!", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Exactly(2));
        }
    }
}