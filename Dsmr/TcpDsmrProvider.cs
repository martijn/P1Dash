using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using P1Dash.Models;

namespace P1Dash.Dsmr;

public class TcpDsmrProvider : IDsmrProvider
{
    private readonly TcpClient? _client;
    private readonly StreamReader? _data;

    public TcpDsmrProvider(IOptions<AppOptions> options)
    {
        var address = options.Value.TcpAddress.Split(":");

        if (address.Length != 2 || !int.TryParse(address[1], out var port))
        {
            Error = "Please specify Tcp Address as host:port in settings.";
            return;
        }

        try
        {
            _client = new TcpClient(address[0], port);
            _data = new StreamReader(new NetworkStream(_client.Client));

            Connected = true;
        }
        catch (SocketException e)
        {
            Error = e.Message;
        }
    }

    public bool Connected { get; }
    public string? Error { get; }

    public P1Telegram? Read()
    {
        return new P1Telegram(FetchMessage());
    }

    private string FetchMessage()
    {
        var message = "";

        if (_data == null) return message;

        do
        {
            var line = _data.ReadLine();

            if (line == null) continue;

            if (line.StartsWith("/") || message.Length > 0) message += line + "\r\n";
        } while (message.LastIndexOf("!") == -1);

        return message;
    }
}
