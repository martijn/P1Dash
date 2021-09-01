namespace P1Dash.Dsmr
{
    public interface IDsmrProvider
    {
        public bool Connected { get; }
        public string? Error { get; }

        public P1Telegram? Read();
    }
}
