using MediatR;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce_API.Database;
using Dapper;
using System.Data;
using Ecommerce_API.Models.Users;
using System.Collections.Generic;

namespace Ecommerce_API.Features.Authentication
{
    public class TokenValidation
    {
        public class Query : IRequest<User>
        {
            public string Token { get; set; }
            public string DeviceToken { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(q => q.Token).Length(50).WithMessage("Malformed Authentication Token");
            }
        }

        public class QueryHandler : IRequestHandler<Query, User>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var userId = await GetUserIdFromToken(db, request);
                    if (userId == null || userId <= 0)
                        return null;

                    var getUserHandler = new Users.GetUsers.QueryHandler(_db);
                    List<User> users = (await getUserHandler.Handle(new Users.GetUsers.Query { Id = (int)userId }, cancellationToken)).AsList();
                    return users[0];
                }
            }

            public async Task<int?> GetUserIdFromToken(IDbConnection db, Query request)
            {
                string isValidTokenQuery = @"UPDATE Tokens SET LastSeen = GETDATE() OUTPUT inserted.UserId WHERE Token=@Token";
                var userId = await db.ExecuteScalarAsync<int?>(isValidTokenQuery, request);
                return userId;
            }
        }
    }
}