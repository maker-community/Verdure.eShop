namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingStockValidationIntegrationEvent(
    Guid OrderId,
    IEnumerable<OrderStockItem> OrderStockItems)
    : IntegrationEvent;
