using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "RabbitSenderApp";

IConnection cnn = factory.CreateConnection();
IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"Sending message {i}");
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello, RabbitMQ message: {i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
    Thread.Sleep(1000);
}

//byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello, RabbitMQ!");
//channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

channel.Close();
cnn.Close();