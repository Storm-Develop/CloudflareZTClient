namespace CloudflareZTClient.Models
{
    /// <summary>
    /// Helper class which contains auth token.
    /// </summary>
    public class OuathDataModel
    {
        public long auth_token { get; set; }

        public override string ToString()
        {
            return $"Auth Token: {auth_token}";
        }
    }
}