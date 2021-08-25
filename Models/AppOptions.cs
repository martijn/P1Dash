using System.ComponentModel.DataAnnotations;

namespace P1Dash.Models
{
    public class AppOptions
    {
        public const string App = "App";
        
        public enum ProviderType : int
        {
            Serial = 1,
            Tcp = 2
        }

        [Required] public ProviderType Provider { get; set; } = ProviderType.Serial;

        public string SerialPort { get; set; } = "/dev/ttyUSB0";
        public string TcpAddress { get; set; } = "localhost:2001";
    }
}
