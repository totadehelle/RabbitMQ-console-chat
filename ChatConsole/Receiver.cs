using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatConsole
{
    public sealed class Receiver :IDisposable
    {
        private static readonly Receiver _instance = new Receiver();
        private EventingBasicConsumer consumer;
        private IConnection connection;
        private IModel channel;

        static Receiver()
        {
        }

        private Receiver()
        {
            Receive();
        }
        
        public static Receiver Instance => _instance;
        
        public void Receive()
        {
            Message deserializedMessage = new Message();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            consumer = new EventingBasicConsumer(channel);

            channel.ExchangeDeclare(exchange: "chat", type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                exchange: "chat",
                routingKey: "");

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                deserializedMessage = JsonConvert.DeserializeObject<Message>(message);
                Console.WriteLine("{0} [{1}]: {2}", deserializedMessage.username, deserializedMessage.date,
                    deserializedMessage.text);
            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }


        private void ReleaseUnmanagedResources()
        {
            connection.Dispose();
            channel.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Receiver()
        {
            ReleaseUnmanagedResources();
        }
    }
}