using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using api.Database;

namespace CRMApi.Features.User
{
    public class GetUsers
    {

        public class Query : IRequest<IEnumerable<api.Models.Users.User>>
        {
            public int Status { get; set; }
            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<api.Models.User>>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<IEnumerable<api.Models.User>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var users = await GetUsers(db, request);
                    return users;
                }
            }

            public async Task<IEnumerable<api.Models.User>> GetUsers(IDbConnection db, Query request)
            {

                string getUsersQuery = @"
                SELECT 
                u.Id,
                first_name + ' ' + last_name AS Name,
                first_name,
                middle_name,
                last_name,
				u.logon_name,
                [Type],
				ut.Name as TypeName,
                job_department AS Department,
				jd.DepartmentName,
                HrCode,
				u.visibleOnMobile,
				u.TVFolder,
				u.Wakeel,
                CASE WHEN sm.Name IS NULL THEN 'بلا' ELSE sm.Name END AS WakeelName,
                u.Mobile,
                u.Email,
				ue.Extention as Extension,
				ue.CCExtetion as CCExtension,

                UserGuid AS [Guid],

				u.is_Locked,
				u.Status,
                CASE WHEN u.Status=1 THEN 'مفعل' ELSE 'مجمد' END AS StatusName
                FROM Users u
				Left JOIN UsersExtentions ue ON ue.UserID = u.ID
				LEFT JOIN JopDepartment jd ON u.job_department = jd.DepartmentId
				LEFT JOIN SalesMan sm ON u.Wakeel = sm.ID
				LEFT JOIN User_Types ut ON u.Type = ut.ID";


                if (request.Status != 0)
                {
                    getUsersQuery += " WHERE u.Status=@Status";
                    if (request.Id > 0)
                        getUsersQuery += " AND u.id=@Id";
                }
                else if (request.Id > 0)
                    getUsersQuery += " WHERE u.id=@Id";

                var users = await db.QueryAsync<api.Models.User>(getUsersQuery, request);
                return users;
            }
        }
    }
}