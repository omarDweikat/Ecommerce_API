using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Users;

namespace Ecommerce_API.Features.Users
{
    public class GetUsers
    {

        public class Query : IRequest<IEnumerable<User>>
        {
            public int Status { get; set; }
            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<User>>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<IEnumerable<User>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var users = await GetUsers(db, request);
                    return users;
                }
            }

            public async Task<IEnumerable<User>> GetUsers(IDbConnection db, Query request)
            {
                string getUsersQuery = @"SELECT * FROM Users";

                if (request.Status != 0)
                {
                    getUsersQuery += " WHERE u.Status=@Status";
                    if (request.Id > 0)
                        getUsersQuery += " AND u.id=@Id";
                }
                else if (request.Id > 0)
                    getUsersQuery += " WHERE u.id=@Id";

                var users = await db.QueryAsync<User>(getUsersQuery, request);
                return users;
            }
        }
    }
}