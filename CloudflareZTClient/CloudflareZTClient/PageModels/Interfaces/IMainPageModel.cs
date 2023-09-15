namespace CloudflareZTClient.Services.Interfaces
{
    using System.Threading.Tasks;
    using CloudflareZTClient.Models;

    public interface IMainPageModel
    {
        //Note this interface needed so we can create unit tests for the main page model.

        /// <summary>
        /// Checking the VPN status connectivity every 5 seconds.
        /// </summary>
        Task CheckVPNStatusAsync();

        /// <summary>
        /// Call to connect to the VPN.
        /// </summary>
        Task ConnectToVPNAsync(bool state);
    }
}
