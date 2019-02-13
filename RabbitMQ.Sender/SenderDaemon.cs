using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitMQ.Sender
{
    public class SenderDaemon : IHostedService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public SenderDaemon(IOptions<SenderDaemonConfig> daemonConfig)
        {
            var factory = new ConnectionFactory
            {
                UserName = daemonConfig.Value.UserName,
                Password = daemonConfig.Value.Password,
                HostName = daemonConfig.Value.HostName,
                Port = daemonConfig.Value.Port,
                VirtualHost = daemonConfig.Value.VirtualHost
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel.QueueDeclare(queue: "HelloWorld",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            Task.Run(() =>
            {
                while (true)
                {
                    _channel.BasicPublish(exchange: "",
                        routingKey: "HelloWorld",
                        basicProperties: null,
                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                    Thread.Sleep(100);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            _channel.Close();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();
        }
    }
}
