using System.ComponentModel.DataAnnotations;

namespace Verdure.eShop.Services.Catalog.API.ViewModel
{
    public class CatalogItemRequest
    {
        [Required]
        public string Name { get;  set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        [Required]
        public string PictureFileName { get;  set; } = string.Empty;
        [Required]
        public string PictureFileId { get;  set; } = string.Empty;
        [Required]
        public string VideoFileId { get;  set; } = string.Empty;

        public int CatalogTypeId { get;  set; }

        public int CatalogBrandId { get;  set; }
    }
}
