using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using System;
using api.Utilities;
using api.Database;

namespace CRMApi.Features.User
{
    public class AddUser
    {
        public class Command : IRequest<Result>
        {
            public api.Models.Users.User user { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            IDatabase _db;
            public CommandHandler(IDatabase db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = new Result();
                var originalUserQuery = "SELECT ID FROM Users WHERE logon_name=@logon_name OR (first_name=@first_name AND middle_name=@middle_name AND last_name=@last_name)";

                using (var db = _db.Open())
                using (var trans = db.BeginTransaction())
                {
                    int originalUserId = await db.QuerySingleOrDefaultAsync<int>(originalUserQuery, request.user, trans);

                    if (originalUserId == 0)
                    {
                        var lastIdQuery = "SELECT MAX(ID) as maxId, MAX(security_object_id) as maxSecId FROM Users";
                        var maximums = await db.QuerySingleOrDefaultAsync<Maximums>(lastIdQuery, request.user, trans);
                        var user = request.user;

                        user.Id = maximums.maxId + 1;
                        user.security_object_id = maximums.maxSecId;
                        user.Guid = Guid.NewGuid();

                        var hashedPass = user.password.Sha512Hash();
                        int? wakeel = 0; if (user.Wakeel != 0) wakeel = user.Wakeel; else wakeel = null;

                        int userId = await db.InsertDynamic("Users", new
                        {
                            ID = user.Id,
                            first_name = user.first_name,
                            middle_name = user.middle_name,
                            last_name = user.last_name, 
                            logon_name = user.logon_name,
                            password = hashedPass,
                            Type = user.Type,
                            job_department = user.Department,
                            HrCode = user.HrCode,
                            visibleOnMobile = user.visibleOnMobile,
                            TVFolder = user.TVFolder,
                            Wakeel = wakeel,
                            Mobile = user.Mobile,
                            Email = user.Email,
                            UserGuid = user.Guid,
                            created_date = DateTime.Now,
                            is_disabled= false
                        }, trans) ?? 0;

                        if (userId == 0)
                        {
                            result.id = user.Id;
                            int extensionId = await db.InsertDynamic("UsersExtentions", new
                            {
                                UserID = user.Id,
                                Extention = user.Extension,
                                CCExtetion = user.CCExtension
                            }, trans) ?? 0;

                            if (extensionId <= 0)
                                result.id = -2;
                        }
                        else
                            result.id = -1;
                        trans.Commit();
                    }
                    else
                    {
                        result.id = -3;
                    }

                }
                return result;
            }
        }
    }
}