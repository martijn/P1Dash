using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using P1Dash.Models;

namespace P1Dash.Dsmr
{
    public class TcpDsmrProvider : IDsmrProvider
    {
        private TcpClient _client;
        private StreamReader _data;
        
        public bool Connected { get; } = false;
        public string? Error { get;  }
        
        public TcpDsmrProvider(IOptions<AppOptions> options)
        {
            var address = options.Value.TcpAddress.Split(":");
            
            Int32 port = 0;
            
            if (address.Length != 2 || !Int32.TryParse(address[1], out port))
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

        public P1Telegram? Read() => new P1Telegram(FetchMessage());

        private string FetchMessage()
        {
            var message = "";

            do
            {
                var line = _data.ReadLine();

                if (line == null) continue;
                
                if (line.StartsWith("/") || message.Length > 0)
                {
                    message += line + "\r\n";
                }
            } while (message.LastIndexOf("!") == -1);

            return message;
        }
    }
}
