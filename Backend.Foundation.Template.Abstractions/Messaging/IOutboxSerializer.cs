namespace Backend.Foundation.Template.Abstractions.Messaging;

public interface IOutboxSerializer
{
    OutboxSerializedMessage Serialize(IIntegrationEvent integrationEvent);
}
