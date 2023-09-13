using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CloudflareZTClient.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;

namespace CloudflareZTClient.Services
{
    public class VPNConnectionService : IVPNConnectionService
    {
        private RestClient _client;
        string client_id = "EHMG4dgdHZZZ09JQLFxj07rVL4jPGvvr.vladsamonintest";
        string client_secret = "Vr7xbZpjed3lXrPD3UP1FaMTjsZHwbGlxDyqTKdMvatHmAlm0bSRkrdfgDg3ziK4";
        string url = "https://vladsamonintest.api.openvpn.com/";
        //"https://vladsamonintest.api.openvpn.com/api/beta/oauth/token?clientid=EHMG4dgdHZZZ09JQLFxj07rVL4jPGvvr.vladsamonintest&client_secret=Vr7xbZpjed3lXrPD3UP1FaMTjsZHwbGlxDyqTKdMvatHmAlm0bSRkrdfgDg3ziK4&grant_type=client_credentials"
        // string url = "https://vladsamonintest.api.openvpn.com/api/beta/oauth/token?client_id=EHMG4dgdHZZZ09JQLFxj07rVL4jPGvvr.vladsamonintest&client_secret=Vr7xbZpjed3lXrPD3UP1FaMTjsZHwbGlxDyqTKdMvatHmAlm0bSRkrdfgDg3ziK4&grant_type=client_credentials";
        public void StartConnection()
        {
            var options = new RestClientOptions(url);

            _client = new RestClient(url);

           GetOauthToken();
            //var response = await _client.ExecuteAsync(request, cancellationToken);
            // SimpleGetRequestTest();
            /*    var restRequest = new RestRequest("/api/beta/regions", Method.Get);

                var result = _client.GetRequestQuery(restRequest);*/
            //var response = await _client.GetJsonAsync<TResponse>("endpoint?foo=bar", cancellationToken);
        }

        public void GetOauthToken()
        {
            string urlRequest = String.Format("api/beta/oauth/token?client_id={0}&client_secret={1}&grant_type=client_credentials",client_id,client_secret);
            var _restRequest = new RestRequest(urlRequest, Method.Post);
            //  _restRequest.RequestFormat = DataFormat.Json;
            //_restRequest.AddHeader("Accept", "application/json");
            //_restRequest.AddHeader("Accept", "application/json");
            //_restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            /// _restRequest.AddParameter("Flow", "application");
           // _restRequest.AddUrlSegment("id", client_id);
         //   _restRequest.AddUrlSegment("secret", client_secret);
           // _restRequest.AddUrlSegment("grant_type", "client_credentials");
            //    _restRequest.AddParameter("client_id", client_id);
            //     _restRequest.AddParameter("client_secret", client_secret);
            var fullUrl = _client.BuildUri(_restRequest);
            var _restResponse = _client.Execute(_restRequest);

            var result = JsonConvert.DeserializeObject<OauthToken>(_restResponse.Content);

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(
                result.access_token, result.token_type
            );

            var options = new RestClientOptions(url)
            {
                Authenticator = authenticator
            };

            _client = new RestClient(options);
            SimpleGetRequestTest();
            //Assert
            Console.WriteLine("**** This is the response **** " + _restResponse.Content);
        }
  
        public void SimpleGetRequestTest()
        {
            //Arrange
            var _restRequest = new RestRequest("/api/beta/regions", Method.Get);
          //  _restRequest.RequestFormat = DataFormat.Json;
//            _restRequest.AddHeader("Accept", "application/json");
            _restRequest.AddHeader("Content-Type", "application/json");
    //        _restRequest.AddHeader("Flow", "application");
        //     _restRequest.AddParameter("client_id", client_id);
        //    _restRequest.AddParameter("client_secret", client_secret);
            //Act
            var _restResponse = _client.Execute(_restRequest);
            var fullUrl = _client.BuildUri(_restRequest);

            var result = JsonConvert.DeserializeObject<Regions>(_restResponse.Content);

            //Assert
            Console.WriteLine("**** This is the response **** " + _restResponse.Content);
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        public class Regions
        {
            [JsonProperty("continent")]
            public string continent { get; set; }

            [JsonProperty("country")]
            public string country { get; set; }

            [JsonProperty("country Iso")]
            public string countryIso { get; set; }

            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("region Name")]
            public string regionName { get; set; }
        }
        public class OauthToken
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }
        }


    }
}
