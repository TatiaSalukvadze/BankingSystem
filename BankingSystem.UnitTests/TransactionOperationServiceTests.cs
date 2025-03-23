using BankingSystem.Application.FacadeServices;
using BankingSystem.Contracts.Interfaces.IServices;
using Moq;

namespace BankingSystem.UnitTests
{
    public class TransactionOperationServiceTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<ICardService> _mockCardService;
        private readonly Mock<ITransactionDetailsService> _mockTransactionDetailsService;
        private readonly TransactionOperationService _transactionOperationService;

        public TransactionOperationServiceTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _mockCardService = new Mock<ICardService>();
            _mockTransactionDetailsService = new Mock<ITransactionDetailsService>();

            _transactionOperationService = new TransactionOperationService(_mockAccountService.Object, _mockCardService.Object, _mockTransactionDetailsService.Object);
        }
    }
}
