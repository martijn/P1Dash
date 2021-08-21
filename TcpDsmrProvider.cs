using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace P1Dash
{
    public class TcpDsmrProvider
    {
        private TcpClient _client;
        private StreamReader _data;
        
        public TcpDsmrProvider()
        {
            _client = new TcpClient("interface.fritz.box", 2001);
            _data = new StreamReader(new NetworkStream(_client.Client));
        }

        public P1Telegram Read()
        {
            var message = FetchMessage();

            var telegram = new P1Telegram();

            var deliveredMatch = Regex.Match(message, @"1-0:1\.7\.0\(([\d\.]+)\*kW\)");

            if (deliveredMatch.Success)
            {
                telegram.ElectricityDelivered = Double.Parse(deliveredMatch.Groups[1].Value);
            }
            else
            {
                Console.WriteLine("Didn't find energy delivered segment in P1 message");
                return null;
            }
            
            var receivedMatch = Regex.Match(message, @"1-0:2\.7\.0\(([\d\.]+)\*kW\)");

            if (receivedMatch.Success)
            {
                telegram.ElectricityReceived = Double.Parse(receivedMatch.Groups[1].Value);
            }
            else
            {
                Console.WriteLine("Didn't find energy received segment in P1 message");
                return null;
            }

            return telegram;
        }
        
        private string FetchMessage()
        {
            // Look for line that starts with /, that is the header
            // Parse all data
            // If line starts with !, stop parsing
            // TODO: check CRC

            var telegram = "";

            do
            {
                var line = _data.ReadLine();

                if (line.StartsWith("/") || telegram.Length > 0)
                {
                    telegram += line + "\n";
                }
            } while (telegram.LastIndexOf("!") == -1);

            return telegram;
        }
    }
}
