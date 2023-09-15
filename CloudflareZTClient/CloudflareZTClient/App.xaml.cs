namespace CloudflareZTClient
{
    using CloudflareZTClient.PageModels;
    using CloudflareZTClient.Services;
    using CloudflareZTClient.Services.Interfaces;
    using FreshMvvm;
    using Xamarin.Forms;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Setting MainPage for the Application
            InitializeDependencyInjection();
            var page = FreshPageModelResolver.ResolvePageModel<MainPageModel>();
            var basicNavContainer = new FreshNavigationContainer(page);
            MainPage = basicNavContainer;
        }

        /// <summary>
        /// Initialize vpn connection service which is going to be used at the main page model.
        /// </summary>
        private void InitializeDependencyInjection()
        {
            FreshIOC.Container.Register<IVPNConnectionService, VPNConnectionService>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
