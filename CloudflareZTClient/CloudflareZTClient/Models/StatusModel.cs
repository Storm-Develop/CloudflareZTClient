namespace CloudflareZTClient.Models
{
    /// <summary>
    /// Used to retrieve daemon data including status, message coming from the networking server.
    /// </summary>
    public class StatusModel
    {
        public string status { get; set; }

        public string message { get; set; }

        public DaemonDataModel data { get; set; }

        public override string ToString()
        {
            return $"Status: {status}, Data: {data.daemon_status}";
        }
    }
}