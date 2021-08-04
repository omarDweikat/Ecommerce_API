using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using System;
using Ecommerce_API.Utilities;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Categories;
using Ecommerce_API.Models.Results;

namespace Ecommerce_API.Features.Categories
{
    public class EditCtegories
    {
        public class Command : IRequest<GeneralResult>
        {
            public Category category { get; set; }
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
                        Category category = request.category;
                        int categoryId = await db.UpdateDynamic("Categories", category, trans, where: "Id=" + category.ID) ?? 0;

                        if (categoryId > 0)
                        {
                            result.ID = categoryId;
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