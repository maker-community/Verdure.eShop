namespace Verdure.eShop.Services.Catalog.API.Model
{
    public class AuditToken
    {

        public string Id { get; private set; } = string.Empty;

        public string CatalogId { get; private set; }

        public string Token { get; private set; }

        public DateTime CreateTime { get; private set; }
        public AuditToken(string catalogId, string token, DateTime createTime)
        {
            CatalogId = catalogId;
            Token = token;
            CreateTime = createTime;
        }
    }
}
