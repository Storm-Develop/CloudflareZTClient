using CloudflareZTClient.Services;
using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudflareZTClient.PageModels
{
    public class MainPageModel : FreshBasePageModel
    {
 #region Default Override functions  

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            var vpnConnection = new VPNConnectionService();
            vpnConnection.StartConnection();
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
        }
#endregion
    }
}
