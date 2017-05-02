using System;
using System.IO;
using System.Text;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProtoBufferConsoleAppExample
{
    public class Rabbit
    {
        public void Publish(string fsSoruce)
        {
            try
            {
                var hostName = "crocodile.rmq.cloudamqp.com"; //"localhost";
                var factory = new ConnectionFactory();
                factory.UserName = "uuwrcbcv";//ConnectionFactory.DefaultUser;
                factory.Password = "EQKfaB21J1IRawGp8F7cJgUQZNoYBmU3"; //ConnectionFactory.DefaultPass;
                factory.VirtualHost = "uuwrcbcv"; //ConnectionFactory.DefaultVHost;
                factory.Protocol = Protocols.DefaultProtocol;
                factory.HostName = hostName;
                factory.Port = AmqpTcpEndpoint.UseDefaultPort;

                var connection = factory.CreateConnection();

                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "AadharBio", type: "direct"); // Need to declare one time before using it at any server side.
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind("hello", "AadharBio", "AADHAR_BIO");

                    byte[] body = Convert.FromBase64String(fsSoruce);

                    channel.BasicPublish(exchange: "AadharBio", routingKey: "AADHAR_BIO", basicProperties: null, body: body);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadLine();
            }
        }

        public void Subscribe()
        {
            Person person = null;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueBind(queue: "hello", exchange: "AadharBio", routingKey: "AADHAR_BIO");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var afterStream = new MemoryStream(body);

                var routingKey = ea.RoutingKey;

                person = Serializer.Deserialize<Person>(afterStream);
                Console.WriteLine($"Id: {person.Id}");
                Console.WriteLine($"Name: {person.Name}");
                Console.WriteLine($"Address1 Line1: {person.Address[0].Line1}");
                Console.WriteLine($"Address1 Line2: {person.Address[0].Line2}");
                Console.WriteLine($"Address2 Line1: {person.Address[1].Line1}");
                Console.WriteLine($"Address2 Line2: {person.Address[1].Line2}");

            };

            channel.BasicConsume(queue: "hello", noAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}