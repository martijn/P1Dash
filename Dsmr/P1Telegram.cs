using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using InvertedTomato.IO;

namespace P1Dash.Dsmr
{
    public class P1Telegram
    {
        public P1Telegram(string message)
        {
            Valid = IsValidMessage(message);

            var versionMatch = Regex.Match(message, @"1-3:0\.2\.8\((\d+)\)");

            if (versionMatch.Success)
            {
                DsmrVersion = int.Parse(versionMatch.Groups[1].Value);
            }
            
            var timestampMatch = Regex.Match(message, @"0-0:1\.0\.0\((\d{12}).\)");

            if (timestampMatch.Success)
            {
                Timestamp = DateTime.ParseExact(timestampMatch.Groups[1].Value, "yyMMddhhmmss", CultureInfo.InvariantCulture);
            }
            
            var deliveredMatch = Regex.Match(message, @"1-0:1\.7\.0\(([\d\.]+)\*kW\)");

            if (deliveredMatch.Success)
            {
                ElectricityDelivered = double.Parse(deliveredMatch.Groups[1].Value);
            }
            
            var receivedMatch = Regex.Match(message, @"1-0:2\.7\.0\(([\d\.]+)\*kW\)");

            if (receivedMatch.Success)
            {
                ElectricityReceived = double.Parse(receivedMatch.Groups[1].Value);
            }
        }
        
        //1-3:0.2.8(50)
        public int DsmrVersion { get; set; }
        
        // 0-0:1.0.0(210830110308S)
        public DateTime Timestamp { get; set; }

        //1-0:1.7.0(00.659*kW)
        public double ElectricityDelivered { get; set; }
        //1-0:2.7.0(00.000*kW)
        public double ElectricityReceived { get; set; }
        
        public bool Valid { get; set; }

        private static bool IsValidMessage(string message)
        {
            var crc = new Crc("CRC-16/DSMR", 16, 0x8005, 0x0000, true, true, 0x0, 0x4B37);

            crc.Append(Encoding.ASCII.GetBytes(Regex.Match(message, pattern: "(/.*?!)", RegexOptions.Singleline).Groups[1].Value));

            return Regex.Match(message, "!(.{4})").Groups[1].Value == crc.ToHexString();
        }
    }
}
