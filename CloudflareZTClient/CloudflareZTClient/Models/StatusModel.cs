namespace CloudflareZTClient.Models
{
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

