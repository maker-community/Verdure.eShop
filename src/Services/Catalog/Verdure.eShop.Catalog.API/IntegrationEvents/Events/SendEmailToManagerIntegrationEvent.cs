namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.Events;

public record SendEmailToManagerIntegrationEvent(
    string CatalogItemId,
    string CatalogItemName,
    string PictureFileName,
    string CatalogItemAuthor,
    string AuditToken)
    : IntegrationEvent;
