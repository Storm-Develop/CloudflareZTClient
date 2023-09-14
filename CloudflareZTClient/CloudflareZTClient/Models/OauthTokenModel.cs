namespace CloudflareZTClient.Models
{
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

