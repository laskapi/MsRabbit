using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;

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


            while (!stoppingToken.IsCancellationRequested)
            {
                var message = string.Format("Worker running at: {0}", DateTime.Now.ToString());
                var body = Encoding.UTF8.GetBytes(message);
                var properties = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "queue", mandatory: true, basicProperties: properties, body: body);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(message);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
