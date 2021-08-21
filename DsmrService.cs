using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace P1Dash
{
    public class DsmrService
    {
        public delegate Task Update(P1Telegram t);
        
        private TcpDsmrProvider _provider;
        private System.Timers.Timer _timer;

        public List<Update> Callbacks = new();

        public DsmrService()
        {
            _provider = new TcpDsmrProvider();
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Interval;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void Interval(object source, ElapsedEventArgs e)
        {
            var telegram = _provider.Read();
            
            foreach (var callback in Callbacks.ToList())
            {
                callback(telegram);
            }
        }
    }
}
