using System;
using RabbitMQ.Client;
using System.Text;

namespace RabbitSender
{
    class Send
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        channel.ExchangeDeclare("direct_logs", "direct");
                        //channel.QueueDeclare(queue: "task_queue", durable: true, 
                        //    exclusive: false, autoDelete: false, arguments: null);
                        var message = $"message {i}";
                        var body = Encoding.UTF8.GetBytes(message);
                        var severity = (args.Length > 0) ? args[0] : "info";
                        Console.WriteLine($"using routing key #{severity}");
                        channel.BasicPublish(exchange: "direct_logs",
                            routingKey: severity,
                            basicProperties: null, body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }


                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
