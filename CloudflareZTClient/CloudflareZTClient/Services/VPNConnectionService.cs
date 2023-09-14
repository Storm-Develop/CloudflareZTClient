using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CloudflareZTClient.PageModels;
using CloudflareZTClient.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using static CloudflareZTClient.Services.VPNConnectionService;

namespace CloudflareZTClient.Services
{
    public class VPNConnectionService : IVPNConnectionService
    {
        private RestClient _client;
        private static string SOCKET_PATH = "/tmp/daemon-lite";
        private OauthTokenModel OauthToken;
        private bool NoErrorContains = true;
        private Socket socketClient;
        private StatusModel daemonStatusModel;

        public StatusModel DaemonStatusModel
        {
            get => this.daemonStatusModel;
            private set
            {
                this.daemonStatusModel = value;
            }
        }
        private string url = "https://warp-registration.warpdir2792.workers.dev/";

        public async Task StartConnectionAsync()
        {
            var options = new RestClientOptions(url);

            _client = new RestClient(url);
            GetOauthToken();
            if (NoErrorContains)
            {
                await ConnectToSocketAsync();
            }
            //CheckStatus();
        }

        public async Task ConnectToVpnAsync()
        {
            if (socketClient != null && socketClient.Connected)
            {
                // Send "connect" request
                await SendRequestAsync(socketClient, new { request = new { connect = OauthToken.data.auth_token } });
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }
        }


        public async Task DisconnectVpnAsync()
        {
            if (socketClient != null && socketClient.Connected)
            {
                // Send "disconnect" request
                await SendRequestAsync(socketClient, new { request = "disconnect" });
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }
        }

        public async Task CheckStatusAsync()
        {
            if (socketClient != null && socketClient.Connected)
            {
                // Send "get_connect" request
                await SendRequestAsync(socketClient, new { request = "get_status" });
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }
        }

        private async Task ConnectToSocketAsync()
        {
            try
            {
                socketClient = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                // Set up the Unix domain socket connection
                var endpoint = new UnixEndPoint(SOCKET_PATH);
                await socketClient.ConnectAsync(endpoint);
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
                var recv_payload = System.Text.Json.JsonSerializer.Deserialize<StatusModel>(recv_payload_json);
                DaemonStatusModel = recv_payload;
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

        public class StatusModel
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

        private void GetOauthToken()
        {
            var _restRequest = new RestRequest("", Method.Get);
            _restRequest.AddHeader("X-Auth-Key", "3735928559");

            var _restResponse = _client.Execute(_restRequest);
            if(!(_restResponse.Content.Contains("error")))
            {
                OauthToken = JsonConvert.DeserializeObject<OauthTokenModel>(_restResponse.Content);
                Console.WriteLine("**** This is the response **** " + OauthToken.data.auth_token);
            }
            else
            {
                NoErrorContains = false;
                Console.WriteLine("Error" + _restResponse.Content);
            }
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
