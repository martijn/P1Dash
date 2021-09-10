using System;

namespace P1Dash.Dsmr
{
    public class DummyDsmrProvider :  IDsmrProvider
    {
        public bool Connected { get; } = true;

        public string? Error { get; } = null;

        private double Balance { get; set; }
        
        private readonly Random _randomGen = new();

        public P1Telegram? Read()
        {
            Balance = Balance - 0.5 + _randomGen.NextDouble();

            double delivered = Balance > 0 ? Balance : 0.0;
            double received = Balance < 0 ? Math.Abs(Balance) : 0.0;

            var template = $@"/Dum5\XS210 ESMR 5.0

1-3:0.2.8(50)
0-0:1.0.0({DateTime.Now:yyMMddHHmmss}S)
0-0:96.1.1(4530303437303030303637323938303139)
1-0:1.8.1(003767.374*kWh)
1-0:1.8.2(003813.352*kWh)
1-0:2.8.1(000028.348*kWh)
1-0:2.8.2(000086.609*kWh)
0-0:96.14.0(0002)
1-0:1.7.0({delivered:00.000}*kW)
1-0:2.7.0({received:00.000}*kW)
0-0:96.7.21(00003)
0-0:96.7.9(00001)
1-0:99.97.0(0)(0-0:96.7.19)
1-0:32.32.0(00001)
1-0:32.36.0(00000)
0-0:96.13.0()
1-0:32.7.0(232.0*V)
1-0:31.7.0(004*A)
1-0:21.7.0(00.618*kW)
1-0:22.7.0(00.000*kW)
0-1:24.1.0(003)
0-1:96.1.0(4730303732303033393331373239373139)
0-1:24.2.1(210910144000S)(02658.345*m3)
!";

            template += P1Telegram.MessageChecksum(template);

            return new P1Telegram(template);
        }
    }
}
