using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using P1Dash.Models;

namespace P1Dash.Dsmr
{
    public class SerialDsmrProvider : IDsmrProvider
    {
        private SerialPort _port;
        
        public bool Connected { get; } = false;
        public string? Error { get; }
        
        public SerialDsmrProvider(IOptions<AppOptions> options)
        {
            try
            {
                _port = new SerialPort(options.Value.SerialPort, 115200);
                _port.ReadTimeout = 1000;
                _port.RtsEnable = true;
                _port.Open();

                Connected = true;
            }
            catch (UnauthorizedAccessException e)
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
                string? line;

                try
                {
                    line = _port.ReadLine();
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout while reading from serial port");
                    line = null;
                }

                if (line == null) continue;
                
                if (line.StartsWith("/") || message.Length > 0)
                {
                    message += line + "\n";
                }
            } while (message.LastIndexOf("!") == -1);

            return message;
        }
    }
}
