using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using InvertedTomato.IO;

namespace P1Dash.Dsmr;

public sealed class P1Telegram
{
    private readonly string _message;

    public P1Telegram(string message)
    {
        _message = message;
    }

    public bool Valid => Regex.Match(_message, "!(.{4})").Groups[1].Value == MessageChecksum(_message);

    public int DsmrVersion => Field<int>("1-3:0.2.8");
    public DateTime Timestamp => Field<DateTime>("0-0:1.0.0");
    public double ElectricityDelivered => Field<double>("1-0:1.7.0");
    public double ElectricityReceived => Field<double>("1-0:2.7.0");
    public double GasDelivered => Field<double>("0-1:24.2.1");
    public double VoltageL1 => Field<double>("1-0:32.7.0");

    public double ElectricityBalance => ElectricityDelivered - ElectricityReceived;

    // Get value of a field by OBIS id in the given value type
    private T Field<T>(string obis) where T : struct
    {
        var value = Regex.Match(_message, @$"{obis}.*\(([\d\.]+).*?\)[\r\n]").Groups[1].Value;

        return typeof(T).Name switch
        {
            nameof(Int32) => (T)(object)int.Parse(value),
            nameof(DateTime) => (T)(object)DateTime.ParseExact(value, "yyMMddHHmmss", CultureInfo.InvariantCulture),
            nameof(Double) => (T)(object)double.Parse(value),
            _ => throw new Exception($"Cannot convert {nameof(T)}")
        };
    }

    public static string MessageChecksum(string message)
    {
        var crc = new Crc("CRC-16/DSMR", 16, 0x8005, 0x0000, true, true, 0x0, 0x4B37);
        var messageBody = Regex.Match(message, "(/.*?!)", RegexOptions.Singleline).Groups[1].Value;

        crc.Append(Encoding.ASCII.GetBytes(messageBody));
        return crc.ToHexString();
    }
}
