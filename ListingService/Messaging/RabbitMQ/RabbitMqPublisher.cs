using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Messaging.RabbitMQ;

public interface IRabbitMqPublisher { Task PublishAsync(object @event); }

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IChannel _channel;
    private readonly string _exchange;

    public RabbitMqPublisher(IChannel channel, IConfiguration config)
    {
        _channel = channel;
        _exchange = config["RabbitMQ:Exchange"]!;
    }

    public async Task PublishAsync(object @event)
    {
        var routingKey = @event.GetType().Name switch
        {
            "ListingCreatedEvent" => "listing.created",
            "ListingUpdatedEvent" => "listing.updated",
            "ListingDeletedEvent" => "listing.deleted",
            _ => throw new InvalidOperationException("Unknown event type")
        };

        var props = new BasicProperties();
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));


        try
        {
            await _channel.BasicPublishAsync(_exchange, routingKey, false, props, body);
        }
        catch(Exception ex)
        {

        }

        
    }
}
