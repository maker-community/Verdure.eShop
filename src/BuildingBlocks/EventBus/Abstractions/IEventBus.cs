namespace Verdure.eShop.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent integrationEvent);
}
