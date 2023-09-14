using CloudflareZTClient.Models;
using CloudflareZTClient.Services;
using FreshMvvm;
using System;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace CloudflareZTClient.PageModels
{
    public class MainPageModel : FreshBasePageModel
    {
        private string vpnStatus;
        private VPNConnectionService vpnConnection;
        private Timer timer;
        private bool isConnectedToVPN;
        private bool connectButtonEnabled;
        private bool disconnectButtonEnabled;
        //Indicates if connection or disconnect in progress
        private bool InProgressOfSendingCommand;
        #region Default Override functions  
        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            vpnConnection = new VPNConnectionService();
            await vpnConnection.StartScoketConnectionAsync();
            await CheckVPNStatus();

            timer = new Timer();

            // Setting up Timer
            timer.Interval = 5000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += CheckVPNStatus;
            timer.Start();

            //Enable connect&disconnect buttons
            DisconnectButtonEnabled = true;
            ConnectButtonEnabled = true;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
            timer.Stop();
        }

        #endregion
        private async void CheckVPNStatus(object sender, ElapsedEventArgs e)
        {
            await CheckVPNStatus();
        }

        private async Task CheckVPNStatus()
        {
            if (!InProgressOfSendingCommand)
            {
                var daemonStatus = await vpnConnection.CheckStatusAsync();

                if (daemonStatus != null)
                {
                    if (daemonStatus.status != null && daemonStatus.status.Equals("success"))
                    {
                        if (daemonStatus.data != null && daemonStatus.data.daemon_status != null)
                        {
                            if (daemonStatus.data.daemon_status.Equals("connected"))
                            {
                                IsConnectedToVPN = true;
                            }
                            else
                            {
                                IsConnectedToVPN = false;
                            }

                            VPNStatus = daemonStatus.data.daemon_status;
                        }
                    }
                    else if (daemonStatus.message != null)
                    {
                        VPNStatus = daemonStatus.message;
                    }
                    else
                    {
                        Console.WriteLine("DaemonStatus Data is null.");
                        VPNStatus = "Connection error";
                    }
                }
                else
                {
                    Console.WriteLine("DaemonStatus is null.");
                    VPNStatus = "Connection error";
                }
            }
        }

        public string VPNStatus
        {
            get => this.vpnStatus;
            set
            {
                this.vpnStatus = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsConnectedToVPN
        {
            get => this.isConnectedToVPN;
            set
            {
                this.isConnectedToVPN = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ConnectButtonEnabled
        {
            get => this.connectButtonEnabled;
            set
            {
                this.connectButtonEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        public bool DisconnectButtonEnabled
        {
            get => this.disconnectButtonEnabled;
            set
            {
                this.disconnectButtonEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        public Command DisconnectVpn
        {
            get
            {
                return new Command(async () =>
                {
                    await ConnectToVPN(false);
                });
            }
        }

        public Command ConnectToVpn
        {
            get
            {
                return new Command(async () =>
                {
                    await ConnectToVPN(true);
                });
            }
        }

        private async Task ConnectToVPN(bool state)
        {
            StatusModel daemonStatus = null;
            InProgressOfSendingCommand = true;
            ConnectButtonEnabled = false;
            DisconnectButtonEnabled = false;

            if (state)
            {
                daemonStatus = await vpnConnection.ConnectToVpnAsync();
            }
            else
            {
                daemonStatus = await vpnConnection.DisconnectVpnAsync();
            }

            if (daemonStatus != null)
            {
                if (daemonStatus.status != null && daemonStatus.status.Equals("success"))
                {
                    if (daemonStatus.data != null && daemonStatus.data.daemon_status != null)
                    {
                        VPNStatus = daemonStatus.data.daemon_status;
                    }
                    IsConnectedToVPN = state;
                }
                else
                {
                    if (daemonStatus.message != null)
                    {
                        VPNStatus = daemonStatus.message;
                    }
                }
            }
            else
            {
                // Handle the case where daemonStatus is null (e.g., a network error occurred)
                VPNStatus = "Connection error";
            }

            ConnectButtonEnabled = true;
            DisconnectButtonEnabled = true;
            InProgressOfSendingCommand = false;
        }
    }
}
