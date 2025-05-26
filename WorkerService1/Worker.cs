using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommonsLib;
namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {

            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "queue", durable: true, exclusive: false,
            autoDelete: false, arguments: null);

            var rnd= new Random();
            while (!stoppingToken.IsCancellationRequested)
            {
                var reading = (double)(rnd.Next(3000, 4000)/100f);
                var model= new RabbitMessageModel { Value= reading,Timestamp=DateTime.Now };
                
                var body =  JsonSerializer.SerializeToUtf8Bytes(model);
                //     var message = string.Format("Date time: {0}, Value: {1}", DateTime.Now,reading.ToString("N2"));
                
                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "queue", mandatory: true, body: body);
                if (_logger.IsEnabled(LogLevel.Information))
                {

                   _logger.LogInformation(model.Timestamp.ToString() +" :: "+ model.Value.ToString());
                }
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
