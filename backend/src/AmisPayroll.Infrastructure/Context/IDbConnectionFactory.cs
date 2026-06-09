using System.Data;

namespace AmisPayroll.Infrastructure.Context
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}