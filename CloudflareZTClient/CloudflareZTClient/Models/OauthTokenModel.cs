namespace CloudflareZTClient.Models
{
    /// <summary>
    /// Used to get OuathToken data from the Rest API call.
    /// </summary>
    public class OauthTokenModel
    {
        public string status { get; set; }

        public OuathDataModel data { get; set; }

        public override string ToString()
        {
            return $"Status: {status}, Data: {data}";
        }
    }
}