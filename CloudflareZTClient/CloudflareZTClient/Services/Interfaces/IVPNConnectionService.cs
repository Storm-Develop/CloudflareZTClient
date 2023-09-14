using System.Threading.Tasks;
using CloudflareZTClient.Models;

namespace CloudflareZTClient.Services.Interfaces
{
    public interface IVPNConnectionService
    {
        Task StartScoketConnectionAsync();
        Task<StatusModel> ConnectToVpnAsync();
        Task<StatusModel> CheckStatusAsync();
        Task<StatusModel> DisconnectVpnAsync();
    }
}
