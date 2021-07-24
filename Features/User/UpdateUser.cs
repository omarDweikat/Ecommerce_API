using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using System;
using api.Utilities;
using api.Database;

namespace CRMApi.Features.User
{
    public class UpdateUser
    {
        public class Command : IRequest<Result>
        {
            public api.Models.User user { get; set; }
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

                using (var db = _db.Open())
                using (var trans = db.BeginTransaction())
                {
                    var user = request.user;
                    int? wakeel = 0; if (user.Wakeel != 0) wakeel = user.Wakeel; else wakeel = null;
                    int userId = await db.UpdateDynamic("Users", new
                    {
                        first_name = user.first_name,
                        middle_name = user.middle_name,
                        last_name = user.last_name,
                        logon_name = user.logon_name,
                        Type = user.Type,
                        job_department = user.Department,
                        HrCode = user.HrCode,
                        visibleOnMobile = user.visibleOnMobile,
                        TVFolder = user.TVFolder,
                        Wakeel = wakeel,
                        Mobile = user.Mobile,
                        Email = user.Email,
                        modified_date = DateTime.Now
                    }, trans, where: "Id=" + user.Id) ?? 0;

                    if (userId > 0)
                    {
                        result.id = userId;
                        if (user.password != null)
                        {
                            var hashedPass = user.password.Sha512Hash();
                            int updatedPass = await db.UpdateDynamic("Users", new
                            {
                                password = hashedPass
                            }, trans, where: "Id=" + user.Id) ?? 0;
                            if (updatedPass <= 0)
                                result.id = -3;

                        }
                        int extensionId = await db.UpdateDynamic("UsersExtentions", new
                        {
                            Extention = user.Extension,
                            CCExtetion = user.CCExtension
                        }, trans, where: "UserID=" + user.Id) ?? 0;

                        if (extensionId <= 0)
                        {
                            int newExtensionId = await db.InsertDynamic("UsersExtentions", new
                            {
                                UserID = user.Id,
                                Extention = user.Extension,
                                CCExtetion = user.CCExtension
                            }, trans) ?? 0;
                            if (newExtensionId <= 0)
                            {
                                result.id = -2;
                            }
                        }
                    }
                    else
                        result.id = -1;
                    trans.Commit();
                }
                return result;
            }
        }
    }
}