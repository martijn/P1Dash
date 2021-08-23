using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using P1Dash.Dsmr;

namespace P1Dash.Services
{
    public class DsmrService
    {
        public delegate Task Update(P1Telegram t);
        
        private IDsmrProvider _provider;
        private System.Timers.Timer _timer;
        private ILogger<DsmrService> _logger;

        public List<Update> Callbacks = new();

        public DsmrService(ILogger<DsmrService> logger)
        {
            _logger = logger;
            try
            {
                _provider = new SerialDsmrProvider();
            }
            catch (System.UnauthorizedAccessException e)
            { 
                _logger.LogInformation("Cannot connect to serial port, falling back to network");
                _provider = new TcpDsmrProvider();
            }
            
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Interval;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void Interval(object? source, ElapsedEventArgs e)
        {
            var telegram = _provider.Read();

            if (telegram == null) return;
            
            foreach (var callback in Callbacks.ToList())
            {
                callback(telegram);
            }
        }
    }
}
