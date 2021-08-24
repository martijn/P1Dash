using System;
using System.Text;
using System.Text.RegularExpressions;
using InvertedTomato.IO;

namespace P1Dash.Dsmr
{
    public class P1Telegram
    {
        public P1Telegram(string message)
        {
            var crc = new Crc("CRC-16/DSMR", 16, 0x8005, 0x0000, true, true, 0x0, 0x4B37);

            crc.Append(Encoding.ASCII.GetBytes(Regex.Match(message, pattern: "(/.*?!)", RegexOptions.Singleline).Groups[1].Value));

            Valid = Regex.Match(message, "!(.{4})").Groups[1].Value == crc.ToHexString();

            var deliveredMatch = Regex.Match(message, @"1-0:1\.7\.0\(([\d\.]+)\*kW\)");

            if (deliveredMatch.Success)
            {
                ElectricityDelivered = Double.Parse(deliveredMatch.Groups[1].Value);
            }
            
            var receivedMatch = Regex.Match(message, @"1-0:2\.7\.0\(([\d\.]+)\*kW\)");

            if (receivedMatch.Success)
            {
                ElectricityReceived = Double.Parse(receivedMatch.Groups[1].Value);
            }
        }
        
        //1-0:1.7.0(00.659*kW)
        public double ElectricityDelivered { get; set; }
        //1-0:2.7.0(00.000*kW)
        public double ElectricityReceived { get; set; }
        
        public bool Valid { get; set; }
    }
}
