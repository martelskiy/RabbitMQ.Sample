using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Receiver
{
    public class ReceiverDaemon : IHostedService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ReceiverDaemon(IOptions<ReceiverDaemonConfig> daemonConfig)
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

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            _channel.BasicConsume(queue: "HelloWorld",
                autoAck: true,
                consumer: consumer);

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
