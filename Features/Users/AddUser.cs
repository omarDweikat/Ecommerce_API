using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using System;
using Ecommerce_API.Utilities;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Users;
using Ecommerce_API.Models.Results;

namespace Ecommerce_API.Features.Users
{
    public class AddUser
    {
        public class Command : IRequest<GeneralResult>
        {
            public User user { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, GeneralResult>
        {
            IDatabase _db;
            public CommandHandler(IDatabase db)
            {
                _db = db;
            }

            public async Task<GeneralResult> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = new GeneralResult();
                try
                {
                    using (var db = _db.Open())
                    using (var trans = db.BeginTransaction())
                    {
                        var lastIdQuery = "SELECT MAX(ID) as maxId FROM Users";
                        int maxID = await db.QuerySingleOrDefaultAsync<int>(lastIdQuery, request.user, trans);
                        User user = request.user;

                        user.ID = maxID;
                        int userId = await db.InsertDynamic("Users", user, trans) ?? 0;

                        if (userId > 0)
                        {
                            result.ID = userId;
                        }
                        else
                        {
                            result.failureMessage = "حدث خلل اثناء اضافة المستخدم";
                        }

                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    result.failureMessage = e.Message;
                }

                return result;
            }
        }
    }
}