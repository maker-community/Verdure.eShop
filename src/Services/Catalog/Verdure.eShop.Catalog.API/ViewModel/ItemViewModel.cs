namespace Verdure.eShop.Services.Catalog.API.ViewModel;

public record ItemViewModel(
    int Id,
    string Name,
    decimal Price,
    string PictureFileName);
