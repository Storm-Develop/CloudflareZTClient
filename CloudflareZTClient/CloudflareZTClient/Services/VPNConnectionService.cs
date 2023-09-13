using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CloudflareZTClient.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;
using System.Net.Sockets;

namespace CloudflareZTClient.Services
{
    public class VPNConnectionService : IVPNConnectionService
    {
        private RestClient _client;
        private static string SOCKET_PATH = "/tmp/daemon-lite";
        private OauthTokenModel OauthToken;

        string url = "https://warp-registration.warpdir2792.workers.dev/";
        //"https://vladsamonintest.api.openvpn.com/api/beta/oauth/token?clientid=EHMG4dgdHZZZ09JQLFxj07rVL4jPGvvr.vladsamonintest&client_secret=Vr7xbZpjed3lXrPD3UP1FaMTjsZHwbGlxDyqTKdMvatHmAlm0bSRkrdfgDg3ziK4&grant_type=client_credentials"
        // string url = "https://vladsamonintest.api.openvpn.com/api/beta/oauth/token?client_id=EHMG4dgdHZZZ09JQLFxj07rVL4jPGvvr.vladsamonintest&client_secret=Vr7xbZpjed3lXrPD3UP1FaMTjsZHwbGlxDyqTKdMvatHmAlm0bSRkrdfgDg3ziK4&grant_type=client_credentials";
        public void StartConnection()
        {
            var options = new RestClientOptions(url);

            _client = new RestClient(url);
            GetOauthToken();
            ConnectToSocket();
            //var response = await _client.ExecuteAsync(request, cancellationToken);
            // SimpleGetRequestTest();
            /*    var restRequest = new RestRequest("/api/beta/regions", Method.Get);

                var result = _client.GetRequestQuery(restRequest);*/
            //var response = await _client.GetJsonAsync<TResponse>("endpoint?foo=bar", cancellationToken);
        }

        private async void ConnectToSocket()
        {
            try
            {
                using (var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified))
                {
                    // Platform-specific code to set up the Unix domain socket connection
                    // You may need to implement this differently for Android and iOS
                    var endpoint = new UnixEndPoint(SOCKET_PATH);
                    client.Connect(endpoint);

                    // Send "get_connect" request
                    await SendRequestAsync(client, new { request = "get_status" });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private async Task SendRequestAsync(Socket client, object requestObject)
        {
            try
            {
                string requestJson = System.Text.Json.JsonSerializer.Serialize(requestObject);
                Console.WriteLine(requestJson);

                // Determine the size of the JSON payload
                long requestSize = Encoding.UTF8.GetByteCount(requestJson);

                // Convert the size of the JSON payload into an 8-byte array (64-bit value)
                byte[] requestSizeBytes = BitConverter.GetBytes(requestSize);

                // Ensure correct endianness for the size field (little-endian on macOS)
                if (BitConverter.IsLittleEndian)
                {
                    // Ensure that requestSizeBytes is exactly 8 bytes long
                    if (requestSizeBytes.Length < 8)
                    {
                        byte[] paddedBytes = new byte[8];
                        Array.Copy(requestSizeBytes, 0, paddedBytes, 0, requestSizeBytes.Length);
                        requestSizeBytes = paddedBytes;
                    }
                }
                else
                {
                    // Convert to little-endian if not already
                    Array.Reverse(requestSizeBytes);
                }
                Console.WriteLine("Sending payload size: " + requestSize);

                // Sending the request size as 8 bytes to the socket
                await SendAllAsync(client, requestSizeBytes);
                Console.WriteLine("Sent payload size.");

                // Then, send the JSON payload to the socket
                await SendAllAsync(client, Encoding.UTF8.GetBytes(requestJson));

                // Reading the response size (8 bytes)
                byte[] recv_payload_size_bytes = await ReceiveAllAsync(client, 8);

                // Ensure correct endianness for the size field (little-endian on macOS)
                if (BitConverter.IsLittleEndian)
                {
                    // Ensure that requestSizeBytes is exactly 8 bytes long
                    if (requestSizeBytes.Length < 8)
                    {
                        byte[] paddedBytes = new byte[8];
                        Array.Copy(requestSizeBytes, 0, paddedBytes, 0, requestSizeBytes.Length);
                        requestSizeBytes = paddedBytes;
                    }
                }
                else
                {
                    // Convert to little-endian if not already
                    Array.Reverse(requestSizeBytes);
                }

                // Convert the 8-byte array into an integer; this is the size of the response payload to read
                int recv_payload_size = BitConverter.ToInt32(recv_payload_size_bytes, 0);

                // Read the JSON payload of the response based on the received size
                byte[] recv_payload_json_bytes = await ReceiveAllAsync(client, recv_payload_size);
                string recv_payload_json = Encoding.UTF8.GetString(recv_payload_json_bytes);

                // Deserialize the JSON payload into a C# object; this is the response
                var recv_payload = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(recv_payload_json);

                Console.WriteLine("Received response: " + recv_payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending/requesting: " + ex.Message);
            }
        }

        private async Task SendAllAsync(Socket socket, byte[] data)
        {
            int totalBytesSent = 0;

            while (totalBytesSent < data.Length)
            {
                int bytesSent = await socket.SendAsync(
                    new ArraySegment<byte>(data, totalBytesSent, data.Length - totalBytesSent),
                    SocketFlags.None
                );
                Console.WriteLine("BytesSent " + bytesSent + " Total Bytes" + totalBytesSent);

                if (bytesSent == 0)
                {
                    throw new Exception("Socket connection closed prematurely.");
                }

                totalBytesSent += bytesSent;
            }
        }

        private async Task<byte[]> ReceiveAllAsync(Socket socket, int length)
        {
            byte[] buffer = new byte[length];
            int totalBytesReceived = 0;

            while (totalBytesReceived < length)
            {
                int bytesReceived = await socket.ReceiveAsync(new ArraySegment<byte>(buffer, totalBytesReceived, length - totalBytesReceived), SocketFlags.None);
                if (bytesReceived == 0)
                {
                    throw new Exception("Socket connection closed prematurely.");
                }
                totalBytesReceived += bytesReceived;
            }

            return buffer;
        }

        public class ResponseModel
        {
            public string status { get; set; }
            public DataModel data { get; set; }

            public override string ToString()
            {
                return $"Status: {status}, Data: {data.daemon_status}";
            }
        }

        public class DataModel
        {
            public string daemon_status { get; set; }

            public override string ToString()
            {
                return $"Daemon Status: {daemon_status}";
            }
        }
        public void GetOauthToken()
        {
           // string urlRequest = String.Format("api/beta/oauth/token?client_id={0}&client_secret={1}&grant_type=client_credentials",client_id,client_secret);
            var _restRequest = new RestRequest("", Method.Get);
            _restRequest.AddHeader("X-Auth-Key", "3735928559");
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
            var _restResponse = _client.Execute(_restRequest);
            Console.WriteLine("**** This is the response **** " + _restResponse.Content);
            var result = JsonConvert.DeserializeObject<OauthTokenModel>(_restResponse.Content);

           // _restRequest = new RestRequest("get_status", Method.Get);
           // _client.Execute(_restRequest);
            //_restRequest.AddHeader("connect", "245346449271196");
            //var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(
            //    result.access_token, result.token_type
            //);

            //var options = new RestClientOptions(url)
            //{
            //    Authenticator = authenticator
            //};

            //_client = new RestClient(options);
         //   SimpleGetRequestTest();
            //Assert
         //   Console.WriteLine("**** This is the response **** " + _restResponse.Content);
        }


        public class OauthTokenModel
        {
            public string status { get; set; }
            public OuathDataModel data { get; set; }

            public override string ToString()
            {
                return $"Status: {status}, Data: {data}";
            }
        }

        public class OuathDataModel
        {
            public long auth_token { get; set; }

            public override string ToString()
            {
                return $"Auth Token: {auth_token}";
            }
        }


    }
}
