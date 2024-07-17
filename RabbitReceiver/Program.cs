using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
var uri = config["Uri:UriString"];

ConnectionFactory factory = new();
factory.Uri = new Uri(uri);
factory.ClientProvidedName = "RabbitReceiverApp";

IConnection cnn = factory.CreateConnection();
IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
    byte[] body = eventArgs.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received message: {message}");

    channel.BasicAck(eventArgs.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();
channel.BasicCancel(consumerTag);
channel.Close();
cnn.Close();