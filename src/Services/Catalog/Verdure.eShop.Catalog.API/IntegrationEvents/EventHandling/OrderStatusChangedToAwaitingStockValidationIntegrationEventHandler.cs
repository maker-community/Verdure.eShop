using MongoDB.Driver;
using Verdure.eShop.Catalog.API;

namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler :
    IIntegrationEventHandler<OrderStatusChangedToAwaitingStockValidationIntegrationEvent>
{
    private readonly EmojisDbContext _context;
    private readonly IEventBus _eventBus;

    public OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler(
        EmojisDbContext context,
        IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task Handle(OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event)
    {
        var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

        foreach (var orderStockItem in @event.OrderStockItems)
        {
            var catalogItem = await _context.CatalogItems.Find(c => c.Id == orderStockItem.ProductId).SingleOrDefaultAsync();

            if (catalogItem != null)
            {
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }
        }

        // Simulate work
        await Task.Delay(3000);

        var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
            ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
            : new OrderStockConfirmedIntegrationEvent(@event.OrderId);

        await _eventBus.PublishAsync(confirmedIntegrationEvent);
    }
}
