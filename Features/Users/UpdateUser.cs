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
    public class UpdateUser
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
                        User user = request.user;
                        int userId = await db.UpdateDynamic("Users", user, trans, where: "Id=" + user.ID) ?? 0;

                        if (userId > 0)
                        {
                            result.ID = userId;
                        }
                        else
                        {
                            result.failureMessage = "حدث خلل في حفظ التغييرات على المستخدم";
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