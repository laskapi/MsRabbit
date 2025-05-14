using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignaRWebApp.Hubs;
using System.Text;
using System.Threading.Channels;

namespace SignalrWebApp.Rabbit
{
    public class RabbitClient
    {

        private readonly ILogger _logger;
        private readonly IHubContext<SignalHub> _signalHub;
        private AsyncEventingBasicConsumer? _consumer;
        private ConnectionFactory? factory;
        private IConnection? connection;
        private IChannel? channel;

        public RabbitClient(ILogger<RabbitClient> logger, IHubContext<SignalHub>  signalHub)
        {

            _logger = logger;
            _signalHub = signalHub;

        }


        public async Task<Task> startClientAsync()
        {


            factory = new ConnectionFactory { HostName = "localhost" };
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();


            await channel.QueueDeclareAsync(queue: "queue", durable: true, exclusive: false, autoDelete: false,
        arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _consumer = new AsyncEventingBasicConsumer(channel);


            Console.WriteLine(" [ccccccccccccccccccc] Waiting for messages.");


            _consumer.ReceivedAsync += async (model, ea) =>
            {
        
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

           
                await _signalHub.Clients.All.SendAsync("Received", message);//SendMEssage("blublu");
                
                _logger.LogInformation($" Message Received {message}");
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

            };

            await channel.BasicConsumeAsync("queue", autoAck: false, consumer: _consumer);

            return Task.CompletedTask;




        }

    }
}