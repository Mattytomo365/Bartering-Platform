using Application.Features.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Messaging.RabbitMQ;

/// <summary>
/// Background worker that keeps the Discovery read model in sync by
/// consuming listing events from RabbitMQ and invoking application
/// commands (via MediatR) to upsert or delete rows in the search table.
/// 
/// Topology (from configuration):
/// - Exchange: RabbitMQ:Exchange (topic, durable)
/// - Queue:    RabbitMQ:Queue (durable), bound with routing key "listing.*"
/// - VHost/Host/Port/User/Pass from RabbitMQ:*
/// </summary>

public class ListingIndexConsumer : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;

    /// <summary>
    /// Creates the consumer; stores DI services and configuration.
    /// Does not open any RabbitMQ connections/channels here.
    /// </summary>
    /// <param name="provider">Root service provider. A new scope is created per message.</param>
    /// <param name="config">Configuration source (reads RabbitMQ:Host, Username, Password, VirtualHost, Port, Exchange, Queue).</param>
    public ListingIndexConsumer(IServiceProvider provider, IConfiguration config)
    {
        _provider = provider;
        _config = config;
    }


    /// <summary>
    /// - Validates required RabbitMQ settings.
    /// - Creates a connection and channel (async).
    /// - Declares the topic exchange (durable), queue (durable), and binds with "listing.*".
    /// - Leaves the channel open for <see cref="ExecuteAsync"/> to consume messages.
    /// </summary>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // read settings
        var host = _config["RabbitMQ:Host"] ?? throw new InvalidOperationException("RabbitMQ:Host is not configured.");
        var username = _config["RabbitMQ:Username"] ?? throw new InvalidOperationException("RabbitMQ:Username is not configured.");
        var password = _config["RabbitMQ:Password"] ?? throw new InvalidOperationException("RabbitMQ:Password is not configured.");
        var exchange = _config["RabbitMQ:Exchange"] ?? throw new InvalidOperationException("RabbitMQ:Exchange is not configured.");
        var queue = _config["RabbitMQ:Queue"] ?? throw new InvalidOperationException("RabbitMQ:Queue is not configured.");
        var virtualHost = _config["RabbitMQ:VirtualHost"] ?? throw new InvalidOperationException("RabbitMQ:VirtualHost is not configured.");
        var portString = _config["RabbitMQ:Port"] ?? throw new InvalidOperationException("RabbitMQ:Port is not configured.");
        if (!int.TryParse(portString, out var port))
            throw new InvalidOperationException("RabbitMQ:Port must be a valid integer.");

        // build the factory
        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
            VirtualHost = virtualHost,
            Port = port
        };

        // create the connection & channel
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // declare exchange, queue, bind
        await _channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );
        await _channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );
        await _channel.QueueBindAsync(
            queue,
            exchange,
            routingKey: "listing.*",
            arguments: null,
            cancellationToken: cancellationToken
        );

        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Main loop of the background service. Subscribes an async consumer to the configured
    /// queue and processes messages until the host is shutting down.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");

        // create the async consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                // decode message body
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                // create a DI scope for this message
                using var scope = _provider.CreateScope();

                // resolve MediatR (and thus the handlers + DbContext) from the scope
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var type = ea.RoutingKey;

                switch (type)
                {
                    case "listing.created":
                    case "listing.updated":
                        var up = JsonSerializer.Deserialize<UpsertListingCommand>(json);
                        if (up != null) await mediator.Send(up, stoppingToken);
                        break;

                    case "listing.deleted":
                        var del = JsonSerializer.Deserialize<SoftDeleteListingCommand>(json);
                        if (del != null) await mediator.Send(del, stoppingToken);
                        break;
                }

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex}");
                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken
                );
            }
        };

        // start consuming
        await _channel.BasicConsumeAsync(
            queue: _config["RabbitMQ:Queue"]!,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        // keep the service alive
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException) { /* graceful exit */ }
    }

    /// <summary>
    /// Attempts a graceful shutdown: closes the RabbitMQ channel and connection, then
    /// defers to the base implementation to complete background service shutdown.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes the RabbitMQ channel and connection resources created during startup.
    /// Safe to call multiple times.
    /// </summary>
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
