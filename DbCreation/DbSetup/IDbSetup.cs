namespace DbCreation.DbSetup
{
    public interface IDbSetup
    {
        Task CreateDbAndTablesAsync();
    }
}
