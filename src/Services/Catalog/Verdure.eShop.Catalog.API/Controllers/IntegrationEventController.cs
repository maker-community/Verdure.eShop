namespace Verdure.eShop.Services.Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "verdure-eshop-pubsub";

    [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToAwaitingStockValidationIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event,
        [FromServices] OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler handler) =>
        handler.Handle(@event);

    [HttpPost("OrderStatusChangedToPaid")]
    [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToPaidIntegrationEvent")]
    public Task HandleAsync(
        OrderStatusChangedToPaidIntegrationEvent @event,
        [FromServices] OrderStatusChangedToPaidIntegrationEventHandler handler) =>
        handler.Handle(@event);

    [HttpPost("SendEmailToManager")]
    [Topic(DAPR_PUBSUB_NAME, "SendEmailToManagerIntegrationEvent")]
    public Task HandleAsync(
    SendEmailToManagerIntegrationEvent @event,
    [FromServices] SendEmailToManagerIntegrationEventHandler handler) =>
    handler.Handle(@event);
}
