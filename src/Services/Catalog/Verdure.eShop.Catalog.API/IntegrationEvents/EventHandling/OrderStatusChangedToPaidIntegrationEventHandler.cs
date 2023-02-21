using MongoDB.Driver;
using Verdure.eShop.Catalog.API;

namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToPaidIntegrationEventHandler :
    IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly EmojisDbContext _context;

    private readonly UpdateDefinitionBuilder<CatalogItem> _bu = Builders<CatalogItem>.Update;

    public OrderStatusChangedToPaidIntegrationEventHandler(EmojisDbContext context)
    {
        _context = context;
    }

    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        //we're not blocking stock/inventory
        foreach (var orderStockItem in @event.OrderStockItems)
        {
            var catalogItem = await _context.CatalogItems.Find(c => c.Id == orderStockItem.ProductId).FirstOrDefaultAsync();
            if (catalogItem != null)
            {
                catalogItem.RemoveStock(orderStockItem.Units);

                await _context.CatalogItems.UpdateOneAsync(c => c.Name == catalogItem.Name,
                    _bu.Set(c => c.AvailableStock, catalogItem.AvailableStock), new() { IsUpsert = true });
            }
        }

    }
}
