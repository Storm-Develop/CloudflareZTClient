namespace CloudflareZTClient.Services
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
    using CloudflareZTClient.Models;
    using CloudflareZTClient.Services.Helper;
    using CloudflareZTClient.Services.Interfaces;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// VPN Connection service which is used to communicate to the daemon server.
    /// </summary>
    public class VPNConnectionService : IVPNConnectionService
    {
        // Rest client used to communicate with cloudfare server to get the ouath token.
        private RestClient restClient;

        // Ouath token timer which is used to generate the new ouath token.
        private Timer oauthTokenTimer;

        private static string SOCKET_PATH = "/tmp/daemon-lite";

        // Url used for grabbing the oauth token.
        private string cloudfareOuathUrl = "https://warp-registration.warpdir2792.workers.dev/";

        // Ouath token.
        private OauthTokenModel OauthToken;

        // Socket client to communicate with the server.
        private Socket socketClient;

        // Daemon status model.
        private StatusModel daemonStatusModel;

        // Ouath error message.
        private string oauthErrorMessage;

        // Boolean indicating if new ouath token is required to be generated.
        private bool NewOauthTokenRequired;

        // Rest request used by rest client to generate new oauth token.
        private RestRequest restRequest;

        /// <summary>
        /// Status model contains the information from the server;
        /// </summary>
        public StatusModel DaemonStatusModel
        {
            get => this.daemonStatusModel;
            private set
            {
                this.daemonStatusModel = value;
            }
        }

        /// <summary>
        /// Shows the error on ouath token request in case it happens.
        /// </summary>
        public string OauthErrorMessage
        {
            get => this.oauthErrorMessage;
            private set
            {
                this.oauthErrorMessage = value;
            }
        }

        /// <summary>
        /// Start socket connection, rest client & timer upon first launch of the app.
        /// </summary>
        /// <returns></returns>
        public async Task StartSocketConnectionAsync()
        {
            this.NewOauthTokenRequired = true;
            this.restClient = new RestClient(cloudfareOuathUrl);
            this.oauthTokenTimer = new Timer();

            // Setting up Timer
            this.oauthTokenTimer.Interval = 300000; // 5 minutes timer which is in the interval time is 300000
            this.oauthTokenTimer.AutoReset = true;
            this.oauthTokenTimer.Enabled = false;
            this.oauthTokenTimer.Elapsed += UpdateOauthTokenFlags;
            this.oauthTokenTimer.Start();

            await ConnectToSocketAsync();
        }

        /// <summary>
        /// Updating Oauth token flags to make sure when 5 minutes are passed the new token is going to be generated on a new connection requst.
        /// </summary>
        private void UpdateOauthTokenFlags(object sender, ElapsedEventArgs e)
        {
            this.NewOauthTokenRequired = true;
            this.oauthTokenTimer.Enabled = false;
        }

        /// <summary>
        /// Sending connect request to the server.
        /// </summary>
        /// <returns>StatusModel containing the infromation from the server.</returns>
        public async Task<StatusModel> ConnectToVpnAsync()
        {
            // Checking if the new oauth token required.
            if (this.NewOauthTokenRequired)
            {
                Console.WriteLine("Requesting a fresh OAuth token.");

                GetOauthToken();

                // In rare cases it could fail to get oauth token from the server, in order to cover it generating proper status model.
                if (this.OauthToken==null || this.OauthToken.data == null)
                {
                    this.DaemonStatusModel = new StatusModel();
                    this.DaemonStatusModel.status = "error";
                    this.DaemonStatusModel.message = "An error occurred while retrieving the OAuth token.";
                    Console.WriteLine("An error occurred while retrieving the OAuth token.");
                }
            }

            if (this.socketClient != null && this.socketClient.Connected && this.OauthToken != null && this.OauthToken.data != null)
            {
                // Send "connect" request
                await SendRequestAsync(this.socketClient, new { request = new { connect = this.OauthToken.data.auth_token } });

                // If connection to the server fails and if the new token required than we make sure that we keep the token and refresh it after 5 minutes.
                // Note new token may not be required if the client fails e.g twice to connect to the server and we already have the token to be refreshed.
                // Also it's going to refresh a token when 5 minutes are passed and user tries to connect again.
                if ((this.DaemonStatusModel ==null || this.DaemonStatusModel.status.Equals("error")) && this.NewOauthTokenRequired)
                {
                    Console.WriteLine("Keep the same Oauth Token for 5 minutes");
                    this.NewOauthTokenRequired = false;
                    this.oauthTokenTimer.Enabled = true;
                }
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }

            return this.DaemonStatusModel;
        }

        /// <summary>
        /// Sending disconnect request to the server.
        /// </summary>
        /// <returns>StatusModel containing the infromation from the server.</returns>
        public async Task<StatusModel> DisconnectVpnAsync()
        {
            if (this.socketClient != null && this.socketClient.Connected)
            {
                // Send "disconnect" request
                await SendRequestAsync(this.socketClient, new { request = "disconnect" });
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }

            return this.DaemonStatusModel;
        }

        /// <summary>
        /// Checking the status of the daemon server by sending "get_status" request.
        /// </summary>
        /// <returns>StatusModel containing the infromation from the server.</returns>
        public async Task<StatusModel> CheckStatusAsync()
        {
            if (this.socketClient != null && this.socketClient.Connected)
            {
                // Send "get_connect" request
                await SendRequestAsync(this.socketClient, new { request = "get_status" });
            }
            else
            {
                Console.WriteLine("Socket is not connected.");
            }

            return this.DaemonStatusModel;
        }

        /// <summary>
        /// Getting the oauth token by sending REST API Call to the cloudfare server.
        /// </summary>
        private void GetOauthToken()
        {
            this.restRequest = new RestRequest(string.Empty, Method.Get);

            // Using a header provided on the assignment.
            this.restRequest.AddHeader("X-Auth-Key", "3735928559");

            var restResponse = this.restClient.Execute(this.restRequest);
            if (!restResponse.Content.Contains("error"))
            {
                // Deserialize object to receive oauth token.
                this.OauthToken = JsonConvert.DeserializeObject<OauthTokenModel>(restResponse.Content);
                Console.WriteLine("**** This is the response **** " + this.OauthToken.data.auth_token);
            }
            else
            {
                var errorMessage = "Error" + restResponse.Content;
                this.OauthErrorMessage = errorMessage;
                Console.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// Connecting to the socket to communicate with the daemon server.
        /// </summary>
        private async Task ConnectToSocketAsync()
        {
            try
            {
                // Constructing socket client.
                this.socketClient = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

                // Set up the Unix domain socket connection
                var endpoint = new UnixEndPoint(SOCKET_PATH);

                //Connect to the socket to the given ednpoint
                await this.socketClient.ConnectAsync(endpoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Send request to the server is used to send all commands and retrieve the response from the server.
        /// </summary>
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

                // Sending the request size as 8 bytes to the socket
                await SendAllAsync(client, requestSizeBytes);

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

                // Setting Daemon status model to be used later.
                this.DaemonStatusModel = recv_payload;
                Console.WriteLine("Received response: " + recv_payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending/requesting: " + ex.Message);
            }
        }

        /// <summary>
        /// Helper function which is sending an array of bytes to the server through the socket.
        /// </summary>
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

        /// <summary>
        /// Helper function which is getting the list of bytes received from the server.
        /// </summary>
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

    }
}
