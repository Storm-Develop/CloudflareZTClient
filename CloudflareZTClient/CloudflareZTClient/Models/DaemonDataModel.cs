namespace CloudflareZTClient.Models
{
    /// <summary>
    /// Helper class to get daemon status.
    /// </summary>
    public class DaemonDataModel
    {
        public string daemon_status { get; set; }

        public override string ToString()
        {
            return $"Daemon Status: {daemon_status}";
        }
    }
}