using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce_API.Database;
using Ecommerce_API.Utilities;
using Dapper;
using FluentValidation;
using MediatR;
using Ecommerce_API.Models.Users;

namespace Ecommerce_API.Features.Authentication
{
    public class Login
    {               

        public class Command : IRequest<User>
        {
            public string Username { 
                get; set; }
                
            public string Password { 
                get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Username).NotNull().NotEmpty().WithMessage("اسم المستخدم اجباري");
                RuleFor(c => c.Password).NotNull().NotEmpty().WithMessage("كلمة المرور اجباري");
            }
        }

        public class CommandHandler : IRequestHandler<Command, User>
        {
            IDatabase _db;
            public CommandHandler(IDatabase db)
            {
                _db = db;
            }

            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var id_password = await GetUserIdAndPassword(db, request.Username);
                    if (id_password == null)
                        return null;

                    string password = id_password.Password;
                    if (password == null || !password.Equals(request.Password))
                    {
                        Serilog.Log.Warning($"Invalid Password: userId: {request.Username} password: {request.Password}");
                        return null;
                    }

                    int userId = id_password.Id;

                    var user = await GetUser(db, userId);
                    user.Token = GenerateToken();
                    await StoreToken(db, user.ID, user.Token, DateTime.MaxValue);
                    return user;
                }
            }

            public async Task<int> StoreToken(IDbConnection db, int userId, string token, DateTime expireDate)
            {
                var now = DateTime.Now;
                var authToken = new
                {
                    UserId = userId,
                    Token = token,
                    AssignDate = now,
                    ExpireDate = expireDate,
                    LastSeen = now
                };
                await db.InsertDynamic("Tokens", authToken);
                return 0;
            }

            public async Task<dynamic> GetUserIdAndPassword(IDbConnection db, string username)
            {
                string getPasswordQuery = @"SELECT Id,Password FROM Users WHERE Username = @username AND Status = 1";
                var id_password = await db.QueryFirstOrDefaultAsync(getPasswordQuery, new { username });
                return id_password;
            }

            public string GenerateToken()
            {
                var token = StringUtil.RandomString(50);
                return token;
            }

            public async Task<User> GetUser(IDbConnection db, int userId)
            {
                string getUserQuery = @"SELECT 
                                        *
                                        FROM Users WHERE id = @userId";
                var user = await db.QueryFirstOrDefaultAsync<User>(getUserQuery, new { userId });
                return user;
            }
        }
    }
}