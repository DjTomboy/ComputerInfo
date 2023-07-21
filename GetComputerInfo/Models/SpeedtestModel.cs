using Newtonsoft.Json;

namespace ComputerInfo.Models
{
    internal class SpeedtestModel
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("ping")]
        public Ping? Ping { get; set; }

        [JsonProperty("download")]
        public Download? Download { get; set; }

        [JsonProperty("upload")]
        public Upload? Upload { get; set; }

        [JsonProperty("packetLoss")]
        public int? PacketLoss { get; set; }

        [JsonProperty("isp")]
        public string? Isp { get; set; }

        [JsonProperty("interface")]
        public Interface? Interface { get; set; }

        [JsonProperty("server")]
        public Server? Server { get; set; }

        [JsonProperty("result")]
        public Result? Result { get; set; }
    }

    public class Download
    {
        [JsonProperty("bandwidth")]
        public int Bandwidth { get; set; }

        [JsonProperty("bytes")]
        public int Bytes { get; set; }

        [JsonProperty("elapsed")]
        public int Elapsed { get; set; }

        [JsonProperty("latency")]
        public Latency? Latency { get; set; }

        public string GetBandwidthInMbps()
        {
            return $"{Math.Round((double)Bandwidth / 125000, 2)} Mbps";
        }
    }

    public class Interface
    {
        [JsonProperty("internalIp")]
        public string? InternalIp { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("macAddr")]
        public string? MacAddr { get; set; }

        [JsonProperty("isVpn")]
        public bool IsVpn { get; set; }

        [JsonProperty("externalIp")]
        public string? ExternalIp { get; set; }
    }

    public class Latency
    {
        [JsonProperty("iqm")]
        public double Iqm { get; set; }

        [JsonProperty("low")]
        public double Low { get; set; }

        [JsonProperty("high")]
        public double High { get; set; }

        [JsonProperty("jitter")]
        public double Jitter { get; set; }

        public string GetIqmString()
        {
            return $"{Math.Round(Iqm, 2)} ms";
        }
    }

    public class Ping
    {
        [JsonProperty("jitter")]
        public double Jitter { get; set; }

        [JsonProperty("latency")]
        public double Latency { get; set; }

        [JsonProperty("low")]
        public double Low { get; set; }

        [JsonProperty("high")]
        public double High { get; set; }

        public string GetLatencyString()
        {
            return $"{Math.Round(Latency, 2)} ms";
        }
    }

    public class Result
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("persisted")]
        public bool Persisted { get; set; }
    }

    public class Server
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("host")]
        public string? Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("location")]
        public string? Location { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("ip")]
        public string? Ip { get; set; }
    }

    internal class Upload
    {
        [JsonProperty("bandwidth")]
        public int Bandwidth { get; set; }

        [JsonProperty("bytes")]
        public int Bytes { get; set; }

        [JsonProperty("elapsed")]
        public int Elapsed { get; set; }

        [JsonProperty("latency")]
        public Latency? Latency { get; set; }

        public string GetBandwidthInMbps()
        {
            return $"{Math.Round((double)Bandwidth / 125000, 2)} Mbps";
        }
    }
}
