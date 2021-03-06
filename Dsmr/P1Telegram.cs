using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InvertedTomato.IO;

namespace P1Dash.Dsmr;

public class P1Telegram
{
    public P1Telegram(string message)
    {
        Valid = IsValidMessage(message);

        // Extract all fields in the telegram by regex
        // Note that this only captures the first value for each OBIS code currently
        foreach (Match match in Regex.Matches(message, @"(\d+-\d+:\d+\.\d+\.\d+).*\(([\d\.]+).*?\)[\r\n]"))
            Fields[match.Groups[1].Value] = match.Groups[2].Value;
    }

    public bool Valid { get; set; }
    public int DsmrVersion => int.Parse(Fields["1-3:0.2.8"]);

    public DateTime Timestamp => DateTime.ParseExact(Fields["0-0:1.0.0"], "yyMMddHHmmss", CultureInfo.InvariantCulture);

    public double ElectricityDelivered => double.Parse(Fields["1-0:1.7.0"]);
    public double ElectricityReceived => double.Parse(Fields["1-0:2.7.0"]);
    public double GasDelivered => double.Parse(Fields["0-1:24.2.1"]);
    public double VoltageL1 => double.Parse(Fields["1-0:32.7.0"]);

    public double ElectricityBalance => ElectricityDelivered - ElectricityReceived;

    private Dictionary<string, string> Fields { get; } = new();

    private static bool IsValidMessage(string message)
    {
        return Regex.Match(message, "!(.{4})").Groups[1].Value == MessageChecksum(message);
    }

    public static string MessageChecksum(string message)
    {
        var crc = new Crc("CRC-16/DSMR", 16, 0x8005, 0x0000, true, true, 0x0, 0x4B37);

        crc.Append(
            Encoding.ASCII.GetBytes(Regex.Match(message, "(/.*?!)", RegexOptions.Singleline).Groups[1].Value));

        return crc.ToHexString();
    }
}
