using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignaRWebApp.Hubs;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using CommonsLib;

namespace SignalrWebApp.Rabbit
{
    public class RabbitClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IHubContext<SignalHub> _signalHub;
        private AsyncEventingBasicConsumer? _consumer;
        private ConnectionFactory? factory;
        private IConnection? connection;
        private IChannel? channel;

        public RabbitClient(ILogger<RabbitClient> logger, IHubContext<SignalHub> signalHub,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _signalHub = signalHub;

        }


        public async Task<Task> startClientAsync()
        {


            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7275/");



            factory = new ConnectionFactory { HostName = "localhost" };
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();


            await channel.QueueDeclareAsync(queue: "queue", durable: true, exclusive: false, autoDelete: false,
        arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _consumer = new AsyncEventingBasicConsumer(channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {

                var bodyByteArray = ea.Body.ToArray();
                RabbitMessageModel message = JsonSerializer.Deserialize<RabbitMessageModel>(Encoding.UTF8.GetString(bodyByteArray))!;
                _logger.LogInformation(message.Timestamp + " :: " + message.Value);

                if (message.Value > 38)
                {
                    using StringContent jsonContent = new(Encoding.UTF8.GetString(bodyByteArray), Encoding.UTF8,
        "application/json");

            
                    using HttpResponseMessage response = await httpClient.PostAsync("",
       jsonContent);

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"{jsonResponse}\n");



                }

                await _signalHub.Clients.All.SendAsync("Received", message?.ToString());
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("queue", autoAck: false, consumer: _consumer);

            return Task.CompletedTask;




        }

    }
}