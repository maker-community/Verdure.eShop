using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Verdure.eShop.Catalog.API;

namespace Verdure.eShop.Services.Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly EmojisDbContext _context;

    public CatalogController(EmojisDbContext context)
    {
        _context = context;
    }


    [HttpPost("Init")]
    public async Task<string> Init()
    {
        var obj = new List<CatalogBrand>()
        {
            new CatalogBrand(1, ".NET"),
            new CatalogBrand(2, "Dapr"),
            new CatalogBrand(3, "Other")
        };
        await _context.CatalogBrands.InsertManyAsync(obj);

        var catalogItems = new List<CatalogItem>()
        {
            new CatalogItem(1, ".NET Bot Black Hoodie", 19.5M, "1.png", "1.zip", 1, 5, 100),
            new CatalogItem(2, ".NET Black & White Mug", 8.5M, "2.png", "2.zip", 1, 2, 100),
            new CatalogItem(3, "Prism White T-Shirt", 12, "3.png", "3.zip", 3, 5, 100),
            new CatalogItem(4, ".NET Foundation T-shirt", 14.99M, "4.png", "4.zip", 1, 5, 100),
            new CatalogItem(5, "Roslyn Red Pin", 8.5M, "5.png", "5.zip", 3, 3, 100),
            new CatalogItem(6, ".NET Blue Hoodie", 12, "6.png", "6.zip", 1, 5, 100),
            new CatalogItem(7, "Roslyn Red T-Shirt", 12, "7.png", "7.zip", 3, 5, 100),
            new CatalogItem(8, "Kudu Purple Hoodie", 8.5M, "8.png", "8.zip", 3, 5, 100),
            new CatalogItem(9, "Cup<T> White Mug", 12, "9.png", "9.zip", 3, 2, 100),
            new CatalogItem(10, ".NET Foundation Pin", 9, "10.png", "10.zip", 1, 3, 100),
            new CatalogItem(11, "Cup<T> Pin", 8.5M, "11.png", "11.zip", 1, 3, 100),
            new CatalogItem(12, "Prism White TShirt", 12, "12.png", "12.zip", 3, 5, 100),
            new CatalogItem(13, "Modern .NET Black & White Mug", 8.5M, "13.png", "13.zip", 1, 2, 100),
            new CatalogItem(14, "Modern Cup<T> White Mug", 12, "14.png", "14.zip", 1, 2, 100),
            new CatalogItem(15, "Dapr Cap", 9.99M, "15.png", "15.zip", 2, 1, 100),
            new CatalogItem(16, "Dapr Zipper Hoodie", 14.99M, "16.png", "16.zip", 2, 5, 100),
            new CatalogItem(17, "Dapr Logo Sticker", 1.99M, "17.png", "17.zip", 2, 4, 100)
        };
        await _context.CatalogItems.InsertManyAsync(catalogItems);

        var catalogType = new List<CatalogType>()
        {
            new CatalogType(1, "Cap"),
            new CatalogType(2, "Mug"),
            new CatalogType(3, "Pin"),
            new CatalogType(4, "Sticker"),
            new CatalogType(5, "T-Shirt")
        };

        await _context.CatalogTypes.InsertManyAsync(catalogType);
        return "ok";
    }

    [HttpGet("brands")]
    [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
    public Task<List<CatalogBrand>> CatalogBrandsAsync()
    {
        return _context.CatalogBrands.Find(Builders<CatalogBrand>.Filter.Empty).ToListAsync();
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
    public Task<List<CatalogType>> CatalogTypesAsync()
    {
        return _context.CatalogTypes.Find(Builders<CatalogType>.Filter.Empty).ToListAsync();
    }


    [HttpGet("items/by_ids")]
    [ProducesResponseType(typeof(List<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<List<CatalogItem>>> ItemsAsync([FromQuery] string ids)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));
            if (numIds.All(nid => nid.Ok))
            {
                var idsToSelect = numIds.Select(id => id.Value);

                var items = (await _context.CatalogItems
                    .Find(Builders<CatalogItem>.Filter.Empty).ToListAsync()).Where(ci => idsToSelect.Contains(ci.Id))
                    .Select(item => new ItemViewModel(
                        item.Id,
                        item.Name,
                        item.Price,
                        item.PictureFileName,
                        item.VideoFileId));

                return Ok(items);
            }
        }

        return BadRequest("Ids value is invalid. Must be comma-separated list of numbers.");
    }

    [HttpGet("items/by_page")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel), (int)HttpStatusCode.OK)]
    public async Task<PaginatedItemsViewModel> ItemsAsync(
        [FromQuery] int typeId = -1,
        [FromQuery] int brandId = -1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 0)
    {
        var query = _context.CatalogItems.AsQueryable();

        if (typeId > -1)
        {
            query = (MongoDB.Driver.Linq.IMongoQueryable<CatalogItem>)query.Where(ci => ci.CatalogTypeId == typeId);
        }

        if (brandId > -1)
        {
            query = (MongoDB.Driver.Linq.IMongoQueryable<CatalogItem>)query.Where(ci => ci.CatalogBrandId == brandId);
        }

        var totalItems = await query
            .CountAsync();

        var itemsOnPage = query
            .OrderBy(item => item.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .Select(item => new ItemViewModel(
                item.Id,
                item.Name,
                item.Price,
                item.PictureFileName,
                item.VideoFileId)).AsEnumerable();

        return new PaginatedItemsViewModel(pageIndex, pageSize, totalItems, itemsOnPage);
    }
}
