using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce_API.Database;

namespace Ecommerce_API.Features.Users
{
    public class GetNextUserId
    {

        public class Query : IRequest<int>
        {
        }

        public class QueryHandler : IRequestHandler<Query, int>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<int> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    int NextUserCode = await getNextUserID(db);
                    return NextUserCode;
                }
            }

            private async Task<int> getNextUserID(IDbConnection db)
            {
                string sqlQuery = " SELECT " +
                                      " MAX ( ID ) + 1 " +
                                      " FROM Users";
                int NextUser = await db.QuerySingleOrDefaultAsync<int>(sqlQuery, null);
                return NextUser;
            }
        }
    }
}