namespace P1Dash.Dsmr
{
    public class P1Telegram
    {
        //1-0:1.7.0(00.659*kW)
        public double ElectricityDelivered { get; set; }
        //1-0:2.7.0(00.000*kW)
        public double ElectricityReceived { get; set; }
    }
}
