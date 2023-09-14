using CloudflareZTClient.Services;
using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace CloudflareZTClient.PageModels
{
    public class MainPageModel : FreshBasePageModel
    {
        private string vpnStatus;
        private string vpnConnectionMessage;
        private VPNConnectionService vpnConnection;
        private System.Timers.Timer timer;
        #region Default Override functions  
        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            vpnConnection = new VPNConnectionService();
            await vpnConnection.StartConnectionAsync();

            timer = new System.Timers.Timer();
            // Setting up Timer
            timer.Interval = 5000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += CheckVPNStatus;
            timer.Start();
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
            timer.Stop();
           // vpnConnection.DisconnectVpn();
        }

        #endregion
        private async void CheckVPNStatus(object sender, ElapsedEventArgs e)
        {
            await vpnConnection.CheckStatusAsync();
            if (vpnConnection.DaemonStatusModel != null && vpnConnection.DaemonStatusModel.data != null)
            {
                Console.WriteLine("STATUS " + vpnConnection.DaemonStatusModel.data.daemon_status);
                VPNStatus = vpnConnection.DaemonStatusModel.data.daemon_status;
            }
            else
            {
                Console.WriteLine("DaemonStatusModel or its data is null.");
                // Handle the case when DaemonStatusModel or its data is null
            }
        }

        public string VPNStatus
        {
            get => this.vpnStatus;
            set
            {
                this.vpnStatus = "Status: " + value;
                this.RaisePropertyChanged();
            }
        }

        public string VPNConnectionMessage
        {
            get => this.vpnConnectionMessage;
            set
            {
                this.vpnConnectionMessage = "Message " + value;
                this.RaisePropertyChanged();
            }
        }

        public Command DisconnectVpn
        {
            get
            {
                return new Command(async () =>
                {
                   await vpnConnection.DisconnectVpnAsync();
                });
            }
        }

        public Command ConnectToVpn
        {
            get
            {
                return new Command(async () =>
                {
                   await vpnConnection.ConnectToVpnAsync();
                });
            }
        }
    }
}
