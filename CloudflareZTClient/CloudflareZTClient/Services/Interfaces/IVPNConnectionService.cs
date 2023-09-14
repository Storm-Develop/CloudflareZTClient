using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CloudflareZTClient.Services.VPNConnectionService;

namespace CloudflareZTClient.Services.Interfaces
{
    public interface IVPNConnectionService
    {
        Task<StatusModel> StartConnectionAsync();
        Task<StatusModel> ConnectToVpnAsync();
        Task<StatusModel> CheckStatusAsync();
        Task<StatusModel> DisconnectVpnAsync();
    }
}
