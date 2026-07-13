
var client = new PipeClient();
await client.ConnectAsync("FlightPathVisualizer");

Console.WriteLine("FlightPathVisualizer is running...");

while (true)
{
    var message = await client.ReceiveAsync();
    if (message != null)
    {
        Console.WriteLine($"Received message: {message}");
    }
}

