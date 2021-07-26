using System.Threading;
using System.Threading.Tasks;
using Ecommerce_API.Database;
using Dapper;
using MediatR;

namespace Ecommerce_API.Features.Authentication
{
    public class Logout
    {
        public class Command : IRequest<Result>
        {
            public int UserId { get; set; }
            public string Token { get; set; }
        }

        public class Result
        {
            public string Message { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            IDatabase _db;
            public CommandHandler(IDatabase db) => _db = db;
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var rows = await db.ExecuteAsync("DELETE FROM Tokens WHERE UserId=@UserId AND Token=@Token", request);
                    return new Result() { Message = rows == 0 ? "Failed" : "Logout Succeeded" };
                }
            }
        }
    }
}