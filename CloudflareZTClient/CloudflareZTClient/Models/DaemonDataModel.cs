using System;
namespace CloudflareZTClient.Models
{
    public class DaemonDataModel
    {
        public string daemon_status { get; set; }

        public override string ToString()
        {
            return $"Daemon Status: {daemon_status}";
        }
    }
}

