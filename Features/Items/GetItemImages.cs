using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Item;
using Ecommerce_API.Models.Image;
namespace Ecommerce_API.Features.Items
{
    public class GetItemImages
    {

        public class Query : IRequest<IEnumerable<Image>>
        {
            public int Status { get; set; }
            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Image>>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<IEnumerable<Image>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var Items = await GetItemImages(db, request);
                    return Items;
                }
            }

            public async Task<IEnumerable<Image>> GetItemImages(IDbConnection db, Query request)
            {
                string GetItemsQuery ="Select *  from Images LEFT JOIN Items on Items.ID=Images.ItemID WHERE Items.ID=Images.ItemID AND Items.ID=@Id";
                var Items = await db.QueryAsync<Image>(GetItemsQuery, request);
                return Items;
            }
       
        }
    }
}