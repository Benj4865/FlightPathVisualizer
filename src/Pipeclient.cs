
using System.IO.Pipes;

public class PipeClient
{
    private NamedPipeClientStream? _pipe;
    private StreamReader? _reader;
    private StreamWriter? _writer;


    public async Task ConnectAsync(string moduleName)
    {
        _pipe = new NamedPipeClientStream(
            ".",
            "MessagePipe",
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        await _pipe.ConnectAsync();

        _reader = new StreamReader(_pipe);
        _writer = new StreamWriter(_pipe)
        {
            AutoFlush = true
        };

        await _writer.WriteLineAsync(moduleName);

    }

    public async Task SendMessage(string type, string recipient, string payload)
    {
        if (_writer == null)
            throw new InvalidOperationException("Not connected.");

        var messageData = new
        {
            type = type,
            recipient = recipient,
            payload = payload
        };

        var message = System.Text.Json.JsonSerializer.Serialize(messageData);

        await _writer.WriteAsync(message);
    }

    public async Task<string?> ReceiveAsync()
    {
        if (_reader == null)
            throw new InvalidOperationException("Not connected.");

        return await _reader.ReadToEndAsync();
    }
}