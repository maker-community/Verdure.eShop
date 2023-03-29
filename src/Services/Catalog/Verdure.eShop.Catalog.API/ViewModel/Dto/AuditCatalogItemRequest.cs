using System.ComponentModel.DataAnnotations;

namespace Verdure.eShop.Services.Catalog.API.ViewModel
{
    public class AuditCatalogItemRequest
    {

        public string CatalogId { get; set; } = string.Empty;
        [Required]
        public string AuditToken { get; set; } = string.Empty;
    }
}
