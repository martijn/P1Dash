using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using P1Dash.Dsmr;

namespace P1Dash.Services;

public class DsmrService
{
    public delegate Task Update(P1Telegram t);

    private readonly IDsmrProvider _provider;
    private readonly Timer _timer;

    private ILogger<DsmrService> _logger;

    public List<Update> Callbacks = new();
    public List<P1Telegram> History = new();

    public DsmrService(ILogger<DsmrService> logger, IDsmrProvider provider)
    {
        _logger = logger;
        _provider = provider;

        _timer = new Timer(1000);
        _timer.Elapsed += Interval;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void Interval(object? source, ElapsedEventArgs e)
    {
        if (!_provider.Connected) return;

        var telegram = _provider.Read();

        if (telegram is not { Valid: true }) return;

        History.Add(telegram);

        if (History.Count > 3660)
            History = History.GetRange(History.Count - 3600, 3600);

        foreach (var callback in Callbacks.ToList()) callback(telegram);
    }
}
