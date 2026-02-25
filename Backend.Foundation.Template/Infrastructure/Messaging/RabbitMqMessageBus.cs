using System.Text;
using System.Text.RegularExpressions;
using Backend.Foundation.Template.Abstractions.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Backend.Foundation.Template.Infrastructure.Messaging;

internal sealed class RabbitMqMessageBus : IMessageBus, IDisposable
{
    private static readonly Regex InvalidRoutingKeyCharacters =
        new("[^a-zA-Z0-9_.-]", RegexOptions.Compiled);

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessageBus> _logger;
    private readonly ConnectionFactory _factory;
    private readonly object _sync = new();

    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqMessageBus(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqMessageBus> logger)
    {
        _options = options.Value;
        _logger = logger;

        ValidateOptions(_options);

        _factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.UserName,
            Password = _options.Password,
            ClientProvidedName = _options.ClientProvidedName,
            AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(Math.Max(1, _options.NetworkRecoveryIntervalSeconds))
        };

        EnsureConnection();
    }

    public Task PublishAsync(
        string? messageId,
        string messageType,
        string payload,
        string? headers = null,
        string? correlationId = null,
        string? causationId = null,
        string? idempotencyKey = null,
        int version = 1,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ct.ThrowIfCancellationRequested();

        EnsureConnection();
        var channel = _channel ?? throw new InvalidOperationException("RabbitMQ channel is not initialized.");

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = messageType;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.MessageId = messageId;
        properties.CorrelationId = correlationId;

        var metadataHeaders = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(headers))
        {
            metadataHeaders["headers_json"] = headers;
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            metadataHeaders["x-correlation-id"] = correlationId;
        }

        if (!string.IsNullOrWhiteSpace(causationId))
        {
            metadataHeaders["x-causation-id"] = causationId;
        }

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            metadataHeaders["x-idempotency-key"] = idempotencyKey;
        }

        metadataHeaders["x-version"] = version;

        if (metadataHeaders.Count > 0)
        {
            properties.Headers = metadataHeaders;
        }

        var body = Encoding.UTF8.GetBytes(payload);
        var routingKey = ResolveRoutingKey(messageType);

        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        lock (_sync)
        {
            DisposeUnsafe();
        }
    }

    private void EnsureConnection()
    {
        lock (_sync)
        {
            if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            {
                return;
            }

            DisposeUnsafe();

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _options.ExchangeName,
                type: _options.ExchangeType,
                durable: true,
                autoDelete: false);

            _logger.LogInformation(
                "RabbitMQ message bus connected to {Host}:{Port}, exchange {Exchange}.",
                _options.HostName,
                _options.Port,
                _options.ExchangeName);
        }
    }

    private string ResolveRoutingKey(string messageType)
    {
        if (!string.IsNullOrWhiteSpace(_options.DefaultRoutingKey))
        {
            return _options.DefaultRoutingKey;
        }

        var safeType = InvalidRoutingKeyCharacters.Replace(messageType, ".");
        return safeType.ToLowerInvariant();
    }

    private void DisposeUnsafe()
    {
        try
        {
            _channel?.Close();
        }
        catch
        {
        }
        finally
        {
            _channel?.Dispose();
            _channel = null;
        }

        try
        {
            _connection?.Close();
        }
        catch
        {
        }
        finally
        {
            _connection?.Dispose();
            _connection = null;
        }
    }

    private static void ValidateOptions(RabbitMqOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.HostName))
        {
            throw new InvalidOperationException("Messaging:RabbitMq:HostName is required.");
        }

        if (options.Port <= 0)
        {
            throw new InvalidOperationException("Messaging:RabbitMq:Port must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(options.ExchangeName))
        {
            throw new InvalidOperationException("Messaging:RabbitMq:ExchangeName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ExchangeType))
        {
            throw new InvalidOperationException("Messaging:RabbitMq:ExchangeType is required.");
        }
    }
}
