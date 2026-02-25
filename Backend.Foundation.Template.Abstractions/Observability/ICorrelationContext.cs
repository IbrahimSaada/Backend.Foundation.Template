namespace Backend.Foundation.Template.Abstractions.Observability;

public interface ICorrelationContext
{
    string? CorrelationId { get; }
}
