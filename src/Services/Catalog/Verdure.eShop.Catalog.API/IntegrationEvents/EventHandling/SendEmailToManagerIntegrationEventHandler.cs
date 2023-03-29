using MongoDB.Driver;
using Verdure.eShop.Catalog.API;
using Verdure.eShop.Services.Catalog.API.Services;

namespace Verdure.eShop.Services.Catalog.API.IntegrationEvents.EventHandling;

public class SendEmailToManagerIntegrationEventHandler :
    IIntegrationEventHandler<SendEmailToManagerIntegrationEvent>
{
    private readonly EmojisDbContext _context;
    private readonly IEmailService _emailService;

    private readonly UpdateDefinitionBuilder<CatalogItem> _bu = Builders<CatalogItem>.Update;

    public SendEmailToManagerIntegrationEventHandler(EmojisDbContext context,IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task Handle(SendEmailToManagerIntegrationEvent @event)
    {
        try
        {
            await _emailService.SendCatalogItemToAuditAsync(@event);
            // todo: 发送审核邮件
        }
        catch (Exception ex)
        {

        }
    }
}
