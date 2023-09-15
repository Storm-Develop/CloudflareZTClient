namespace CloudflareZTClient.Services.Interfaces
{
    using System.Threading.Tasks;
    using CloudflareZTClient.Models;

    /// <summary>
    /// Interface of the VPN Connection service.
    /// </summary>
    public interface IVPNConnectionService
    {
        /// <summary>
        /// Open up socket connection for further use when communicating with the network server.
        /// </summary>
        Task StartSocketConnectionAsync();

        /// <summary>
        /// Connect to the server command which is attempting to connect to the network server.
        /// </summary>
        /// <returns>Status Model which contains information received from the server.</returns>
        Task<StatusModel> ConnectToVpnAsync();

        /// <summary>
        /// Getting the current status from the server.
        /// </summary>
        /// <returns>Status Model which contains information received from the server.</returns>
        Task<StatusModel> CheckStatusAsync();

        /// <summary>
        /// Disconnect from the VPN call.
        /// </summary>
        /// <returns>Status Model which contains information received from the server.</returns>
        Task<StatusModel> DisconnectVpnAsync();
    }
}
