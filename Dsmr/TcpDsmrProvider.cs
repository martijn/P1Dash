using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace P1Dash.Dsmr
{
    public class TcpDsmrProvider : IDsmrProvider
    {
        private TcpClient _client;
        private StreamReader _data;
        
        public TcpDsmrProvider()
        {
            _client = new TcpClient("interface.fritz.box", 2001);
            _data = new StreamReader(new NetworkStream(_client.Client));
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
