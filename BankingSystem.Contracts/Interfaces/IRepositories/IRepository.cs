using System.Data;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IRepository
    {
        void SetTransaction(IDbTransaction transaction);
    }
}
