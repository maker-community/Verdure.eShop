namespace Verdure.eShop.Services.Catalog.API.Services
{
    public interface IEmailService
    {
        Task SendCatalogItemToAuditAsync(SendEmailToManagerIntegrationEvent eventData);
    }
}
