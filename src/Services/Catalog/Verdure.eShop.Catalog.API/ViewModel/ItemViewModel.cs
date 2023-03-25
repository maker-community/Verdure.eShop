namespace Verdure.eShop.Services.Catalog.API.ViewModel;

public record ItemViewModel(
    string Id,
    string Name,
    string Desc,
    decimal Price,
    string PictureFileName,
    string PictureFileId,
    string VideoFileId);
