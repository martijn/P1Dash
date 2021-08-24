using System;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace P1Dash.Dsmr
{
    public class SerialDsmrProvider : IDsmrProvider
    {
        private SerialPort _port;
        
        public SerialDsmrProvider()
        {
            _port = new SerialPort("/dev/ttyUSB0", 115200);
            //_port = new SerialPort("/dev/cu.usbserial-A640HB4X", 115200);
            _port.ReadTimeout = 1000;
            _port.RtsEnable = true;
            _port.Open();
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
                catch (TimeoutException e)
                {
                    Console.WriteLine("Timeout while reading from serial port");
                    line = null;
                }

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
