using System.Text;
using FoodOnline.Core.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FoodOnline.Core.MessageBroker;

public class Publisher : IDisposable
{
    private readonly IConnection _connection;
    
    public Publisher(RabbitMqConfigs rabbitMqConfigs)
    {
        _connection = new ConnectionFactory
        {
            HostName = rabbitMqConfigs.Host,
            Port = rabbitMqConfigs.Port,
            UserName = rabbitMqConfigs.Username,
            Password = rabbitMqConfigs.Password,
        }.CreateConnection();
    }

    public void Publish(string exchange, string routingKey, string body)
    {
        using var channel = _connection.CreateModel();
        {
            channel.BasicPublish(exchange, routingKey, null, Encoding.UTF8.GetBytes(body));
        }
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }
}