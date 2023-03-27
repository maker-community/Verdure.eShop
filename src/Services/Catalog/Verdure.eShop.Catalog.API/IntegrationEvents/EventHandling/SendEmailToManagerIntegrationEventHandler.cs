using MongoDB.Driver;
using Verdure.eShop.Catalog.API;

namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.EventHandling;

public class SendEmailToManagerIntegrationEventHandler :
    IIntegrationEventHandler<SendEmailToManagerIntegrationEvent>
{
    private readonly EmojisDbContext _context;

    private readonly UpdateDefinitionBuilder<CatalogItem> _bu = Builders<CatalogItem>.Update;

    public SendEmailToManagerIntegrationEventHandler(EmojisDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SendEmailToManagerIntegrationEvent @event)
    {
        // todo: 发送审核邮件

    }
}
