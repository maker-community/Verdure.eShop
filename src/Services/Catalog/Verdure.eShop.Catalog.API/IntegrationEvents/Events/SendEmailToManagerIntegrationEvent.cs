namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.Events;

public record SendEmailToManagerIntegrationEvent(
    string CatalogItemId,
    string CatalogItemName,
    string CatalogItemAuthor)
    : IntegrationEvent;
