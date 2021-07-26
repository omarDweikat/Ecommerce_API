using System.Data;

namespace Ecommerce_API.Database
{
    public interface IDatabase
    {
        IDbConnection Open();
    }
}