namespace Server.Core
{
    public sealed class ServerConfig
    {
        //null 혹은 빈 문자열 일시 IPAddress.Any 활용. 값을 넣고 싶다면 ServerConfig.json 에 "123.0.0.1" 등 문자열을 작성하면 ok
        public string CSListenIPAddress { get; set; }
        public int CSListenPort { get; set; }
        public int TimeSlicePerUpdateMSec { get; set; }
        public int NumberOfCSBacklogSockets { get; set; }
    }
}
