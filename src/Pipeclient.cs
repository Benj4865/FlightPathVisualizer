
using System.IO.Pipes;

public class TartaMessage(string type, string subCategory, string sender, string recipient, string payload) : EventArgs
{
    public string Type { get; } = type;
    public  string SubCategory { get; } = subCategory;
    public string Sender { get; } = sender;
    public string Recipient { get; } = recipient;
    public string Payload { get; } = payload;
}
public class PipeClient
{
    private NamedPipeClientStream? _pipe;
    private StreamWriter? _writer;

    public event EventHandler<TartaMessage>? MessageReceived;


    public async Task ConnectAsync(string moduleName)
    {
        _pipe = new NamedPipeClientStream(
            ".",
            "TartaMessagePipe",
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await _pipe.ConnectAsync();

        _writer = new StreamWriter(_pipe)
        {
            AutoFlush = true
        };

        await _writer.WriteLineAsync(moduleName);

        using var reader = new StreamReader(_pipe);
        while (true)
        {

            var recievedLine = await reader.ReadLineAsync();
            if (recievedLine == null)
            {
                continue;
            }
            try
            {
                // Extracting the message propertier and putting them into an object
                var receivedMessage = System.Text.Json.JsonSerializer.Deserialize<TartaMessage>(recievedLine);
                //Invoking the eventhandler by raising and event
                MessageReceived?.Invoke(this, receivedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }

    public async Task SendMessage(string type, string subCategory, string sender, string recipient, string payload)
    {
        if (_writer == null)
            throw new InvalidOperationException("Not connected.");

        var message = new TartaMessage(type, subCategory, sender, recipient, payload.Replace("\n", "").Replace("\r", ""));
        var json_formatted_message = System.Text.Json.JsonSerializer.Serialize(message);
        await _writer.WriteLineAsync(json_formatted_message);
    }
}