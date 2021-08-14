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

        public class Query : IRequest<IEnumerable<GalleryItem>>
        {
            public int Status { get; set; }
            public int Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<GalleryItem>>
        {
            IDatabase _db;
            public QueryHandler(IDatabase db) => _db = db;

            public async Task<IEnumerable<GalleryItem>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var db = _db.Open())
                {
                    var Items = await GetItem(db, request);
                    return Items;
                }
            }
            public async Task<IEnumerable<GalleryItem>> GetItem(IDbConnection db, Query request)
            {
                string getcategoriesQuery = @"select Items.*,Images.Filename from Items  
                        Left join Images ON Items.Id=Images.ItemID AND Images.cover = 1 WHERE Items.ID=Images.ItemID AND Items.ID=@Id";

                var categories = await db.QueryAsync<GalleryItem>(getcategoriesQuery, request);
                return categories;
            }
        }
    }
}