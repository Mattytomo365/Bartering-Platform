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

public class ListingIndexConsumer : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;

    public ListingIndexConsumer(IServiceProvider provider, IConfiguration config)
    {
        _provider = provider;
        _config = config;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // 1) Read settings
        var host = _config["RabbitMQ:Host"] ?? throw new InvalidOperationException("RabbitMQ:Host is not configured.");
        var username = _config["RabbitMQ:Username"] ?? throw new InvalidOperationException("RabbitMQ:Username is not configured.");
        var password = _config["RabbitMQ:Password"] ?? throw new InvalidOperationException("RabbitMQ:Password is not configured.");
        var exchange = _config["RabbitMQ:Exchange"] ?? throw new InvalidOperationException("RabbitMQ:Exchange is not configured.");
        var queue = _config["RabbitMQ:Queue"] ?? throw new InvalidOperationException("RabbitMQ:Queue is not configured.");
        var virtualHost = _config["RabbitMQ:VirtualHost"] ?? throw new InvalidOperationException("RabbitMQ:VirtualHost is not configured.");
        var portString = _config["RabbitMQ:Port"] ?? throw new InvalidOperationException("RabbitMQ:Port is not configured.");
        if (!int.TryParse(portString, out var port))
            throw new InvalidOperationException("RabbitMQ:Port must be a valid integer.");

        // 2) Build the factory
        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
            VirtualHost = virtualHost,
            Port = port
        };

        // 3) Create the connection & channel
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // 4) Declare exchange, queue, bind
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");

        // 5) Create the async consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                // 1) Create a DI scope for this message
                using var scope = _provider.CreateScope();

                // 2) Resolve MediatR (and thus the handlers + DbContext) from the scope
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

        // 6) Start consuming
        await _channel.BasicConsumeAsync(
            queue: _config["RabbitMQ:Queue"]!,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        // 7) Keep the service alive
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException) { /* graceful exit */ }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
