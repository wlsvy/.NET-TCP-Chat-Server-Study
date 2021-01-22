namespace Client
{
    public sealed class ClientConfig
    {
        public string ServerIPAddress { get; set; }
        public int ServerPort { get; set; }
        public int TimeSlicePerUpdateMSec { get; set; }

    }
}
