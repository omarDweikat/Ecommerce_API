using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Categories;

namespace Ecommerce_API.Features.Categories
{
    public class Getcategories
    {

        public class Query : IRequest<IEnumerable<Category>>
        {
            public int Status { get; set; }
            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Category>>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<IEnumerable<Category>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var categories = await Getcategories(db, request);
                    return categories;
                }
            }
            public async Task<IEnumerable<Category>> Getcategories(IDbConnection db, Query request)
            {
                string getcategoriesQuery = @"SELECT * FROM Categories ";

                if (request.Status != 0)
                {
                    getcategoriesQuery += " WHERE Status=@Status";
                    if (request.Id > 0)
                        getcategoriesQuery += " AND id=@Id";
                }
                else if (request.Id > 0)
                    getcategoriesQuery += " WHERE id=@Id";

                var categories = await db.QueryAsync<Category>(getcategoriesQuery, request);
                return categories;
            }
        }
    }
}