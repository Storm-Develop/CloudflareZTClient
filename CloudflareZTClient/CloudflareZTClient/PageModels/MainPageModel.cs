namespace CloudflareZTClient.PageModels
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;
    using CloudflareZTClient.Models;
    using CloudflareZTClient.Services.Interfaces;
    using FreshMvvm;
    using Xamarin.Forms;

    /// <summary>
    /// This is Main Page Model which is connected to MainPage through the FreshMVVM.
    /// </summary>
    public class MainPageModel : FreshBasePageModel , IMainPageModel
    {
        // VPN Status is a major indicator in the app highliting if the server is connected/disconnected or there's an error message.
        private string vpnStatus;

        // Timer used to check status of the server.
        private Timer checkVPNStatusTimer;

        // Boolean indicating if the client is connected to vpn.
        private bool isConnectedToVPN;

        // Indicates if connect button is visible.
        private bool connectButtonEnabled;

        // Indicates if disconnect button is visible.
        private bool disconnectButtonEnabled;

        //Indicates if connection or disconnect in progress
        private bool inProgressOfSendingCommand;

        // VPN Connection service interface used for dependency injection.
        private readonly IVPNConnectionService vpnConnectionService;

        /// <summary>
        /// Main Page model constructor.
        /// </summary>
        /// <param name="vpnConnectionService">Used mock VPNService for NUnit tests and real one for the runtime.</param>
        public MainPageModel(IVPNConnectionService vpnConnectionService)
        {
            this.vpnConnectionService = vpnConnectionService;
        }

        #region Default Override functions
        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            //Openning up the socket connection upon opening the app.
            await vpnConnectionService.StartSocketConnectionAsync();

            //Check VPN status on a first lopad
            await CheckVPNStatusAsync();

            this.checkVPNStatusTimer = new Timer();

            // Setting up Timer
            this.checkVPNStatusTimer.Interval = 5000;
            this.checkVPNStatusTimer.AutoReset = true;
            this.checkVPNStatusTimer.Enabled = true;
            this.checkVPNStatusTimer.Elapsed += CheckVPNStatus;
            this.checkVPNStatusTimer.Start();

            //Enable connect&disconnect buttons
            this.DisconnectButtonEnabled = true;
            this.ConnectButtonEnabled = true;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            //Stop timer upon closing the app.
            checkVPNStatusTimer.Stop();
        }

        #endregion

        /// <summary>
        /// VPN Status is a major indicator in the app highliting if the server is connected/disconnected or there's an error message.
        /// </summary>
        public string VPNStatus
        {
            get => this.vpnStatus;
            set
            {
                this.vpnStatus = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Indicates if the client is connected to vpn.
        /// </summary>
        public bool IsConnectedToVPN
        {
            get => this.isConnectedToVPN;
            set
            {
                this.isConnectedToVPN = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Indicates the state of connect button if it's enabled or disabled.
        /// The app toggles visibility of Connect or Disconnect button based on the connection to the network server.
        /// </summary>
        public bool ConnectButtonEnabled
        {
            get => this.connectButtonEnabled;
            set
            {
                this.connectButtonEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Indicates the state of diconnect button if it's enabled or disabled.
        /// </summary>
        public bool DisconnectButtonEnabled
        {
            get => this.disconnectButtonEnabled;
            set
            {
                this.disconnectButtonEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Disconnect button used to bind disconnect call to the UI layer and getting called from MainPage.xaml.
        /// </summary>
        public Command DisconnectVpn
        {
            get
            {
                return new Command(async () =>
                {
                    await ConnectToVPNAsync(false);
                });
            }
        }

        /// <summary>
        /// Disconnect button used to bind connect call to the UI layer and getting called from MainPage.xaml.
        /// </summary>
        public Command ConnectToVpn
        {
            get
            {
                return new Command(async () =>
                {
                    await ConnectToVPNAsync(true);
                });
            }
        }

        private async void CheckVPNStatus(object sender, ElapsedEventArgs e)
        {
            await CheckVPNStatusAsync();
        }

        /// <summary>
        /// Checking VPN status connection which is called by timer every 5 seconds or on a start at the ViewAppearing call.
        /// </summary>
        public async Task CheckVPNStatusAsync()
        {
            //This bool check needed to make sure we dont create race condtion when the client sends & waits for the Connect or Disconnect commands to be send to the server.
            if (!inProgressOfSendingCommand)
            {
                var daemonStatus = await vpnConnectionService.CheckStatusAsync();

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

        /// <summary>
        /// Connect or Disconnect action sends to the service depending on the boolean state.
        /// </summary>
        /// <param name="state">Indicates if connect or disconnect should happen.</param>
        public async Task ConnectToVPNAsync(bool state)
        {
            StatusModel daemonStatus = null;
            this.inProgressOfSendingCommand = true;
            this.ConnectButtonEnabled = false;
            this.DisconnectButtonEnabled = false;

            if (state)
            {
                daemonStatus = await this.vpnConnectionService.ConnectToVpnAsync();
            }
            else
            {
                daemonStatus = await this.vpnConnectionService.DisconnectVpnAsync();
            }

            if (daemonStatus != null)
            {
                if (daemonStatus.status != null && daemonStatus.status.Equals("success"))
                {
                    if (daemonStatus.data != null && daemonStatus.data.daemon_status != null)
                    {
                        this.VPNStatus = daemonStatus.data.daemon_status;
                    }

                    this.IsConnectedToVPN = state;
                }
                else
                {
                    if (daemonStatus.message != null)
                    {
                        this.VPNStatus = daemonStatus.message;
                    }
                }
            }
            else
            {
                // Handle the case where daemonStatus is null (e.g., a network error occurred)
                this.VPNStatus = "Connection error";
            }

            this.ConnectButtonEnabled = true;
            this.DisconnectButtonEnabled = true;
            this.inProgressOfSendingCommand = false;
        }
    }
}
