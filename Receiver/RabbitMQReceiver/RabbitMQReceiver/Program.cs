using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQReceiver
{
    class Receive
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("direct_logs", "direct");
                //channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false,
                //    arguments: null);
                var queueName = channel.QueueDeclare().QueueName;
                var routingKey = args[0];
                Console.WriteLine($"using routing key #{routingKey}");
                channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: routingKey);

                Console.WriteLine(" [*] Waiting for logs.");
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);


                };
                channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
