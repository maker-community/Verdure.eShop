namespace Verdure.eShop.Services.Catalog.API.Model;

public class CatalogItem
{
    public string Id { get; private set; } = string.Empty;

    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public string PictureFileName { get; private set; }
    public string PictureFileId { get; private set; }
    public string VideoFileId { get; private set; }

    public int CatalogTypeId { get; private set; }

    public CatalogType CatalogType { get; private set; } = null!;

    public int CatalogBrandId { get; private set; }

    public CatalogBrand CatalogBrand { get; private set; } = null!;

    public int AvailableStock { get; private set; }

    public CatalogItem(
        string name,
        decimal price,
        string pictureFileName,
        string pictureFileId,
        string videoFileId,
        int catalogTypeId,
        int catalogBrandId,
        int availableStock)
    {
        Name = name;
        Price = price;
        PictureFileName = pictureFileName;
        PictureFileId = pictureFileId;
        VideoFileId = videoFileId;
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        AvailableStock = availableStock;
    }

    /// <summary>
    /// Simply decrement the quantity of a particular item in inventory.
    /// We don't care if we run out of stock.
    /// </summary>
    public int RemoveStock(int quantityDesired)
    {
        AvailableStock -= quantityDesired;

        return quantityDesired;
    }
}
