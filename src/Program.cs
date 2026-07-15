var client = new PipeClient();
client.MessageReceived += (sender, message) =>
{
    if (message != null)
    {
        Console.WriteLine($"Received message: Type={message.Type}, Subcategory={message.SubCategory}, Sender={message.Sender}, Recipient={message.Recipient}, Payload={message.Payload}");
    }
};

_= client.ConnectAsync("FlightPathVisualizer");

Console.WriteLine("FlightPathVisualizer is running...");


var input = Console.ReadLine();
