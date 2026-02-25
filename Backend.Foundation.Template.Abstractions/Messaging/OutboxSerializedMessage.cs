namespace Backend.Foundation.Template.Abstractions.Messaging;

public sealed record OutboxSerializedMessage(
    Guid MessageId,
    string MessageType,
    string Payload,
    string? Headers,
    DateTime OccurredAtUtc,
    string? CorrelationId,
    string? CausationId,
    string? IdempotencyKey,
    int Version);
