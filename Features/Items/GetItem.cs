using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Item;

namespace Ecommerce_API.Features.Items
{
    public class GetItem
    {

        public class Query : IRequest<IEnumerable<Item>>
        {
            public int Status { get; set; }
            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Item>>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<IEnumerable<Item>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var Items = await GetItem(db, request);
                    return Items;
                }
            }
            public async Task<IEnumerable<Item>> GetItem(IDbConnection db, Query request)
            {
                string getcategoriesQuery = @"SELECT * FROM Items ";

                if (request.Status != 0)
                {
                    getcategoriesQuery += " WHERE Status=@Status";
                    if (request.Id > 0)
                        getcategoriesQuery += " AND id=@Id";
                }
                else if (request.Id > 0)
                    getcategoriesQuery += " WHERE id=@Id";

                var categories = await db.QueryAsync<Item>(getcategoriesQuery, request);
                return categories;
            }
        }
    }
}