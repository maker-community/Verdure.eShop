using MongoDB.Driver;
using Verdure.eShop.Mongo;
namespace Verdure.eShop.Catalog.API;

public sealed class EmojisDbContext : BaseDbContext
{
    /// <summary>
    /// 栏目
    /// </summary>
    public IMongoCollection<CatalogItem> CatalogItems => Database.GetCollection<CatalogItem>("CatalogItem");
    /// <summary>
    /// 品牌
    /// </summary>
    public IMongoCollection<CatalogBrand> CatalogBrands => Database.GetCollection<CatalogBrand>("CatalogBrand");
    /// <summary>
    /// 类型
    /// </summary>
    public IMongoCollection<CatalogType> CatalogTypes => Database.GetCollection<CatalogType>("CatalogType");

    /// <summary>
    /// 审核token
    /// </summary>
    public IMongoCollection<AuditToken> AuditTokens => Database.GetCollection<AuditToken>("AuditToken");
}