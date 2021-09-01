using System.ComponentModel.DataAnnotations;

namespace P1Dash.Models
{
    public class AppOptions
    {
        public enum ProviderType
        {
            Serial = 1,
            Tcp = 2
        }

        public const string App = "App";

        [Required] public ProviderType Provider { get; set; } = ProviderType.Serial;

        public string SerialPort { get; set; } = "/dev/ttyUSB0";
        public string TcpAddress { get; set; } = "localhost:2001";
    }
}
