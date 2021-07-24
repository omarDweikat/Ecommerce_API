using System.Data;

namespace api.Database
{
    public interface IDatabase
    {
        IDbConnection Open();
    }
}