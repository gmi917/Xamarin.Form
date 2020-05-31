using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ARGuide
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowPointLog : ContentPage
	{
        string userTotalPoint = "0";
        public ShowPointLog ()
		{
			InitializeComponent ();
            Title = "兌換積數紀錄";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#ff8400");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
            {                
                showTotalPoint();                                            
            }            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            PopupNavigation.Instance.PushAsync(new PopUp());            
        }       

        public async void showTotalPoint()
        {
            GetUserTotalPoint getUserPoint = new GetUserTotalPoint();
            userTotalPoint = await getUserPoint.getUserTotalPoint(Xamarin.Forms.Application.Current.Properties["email"].ToString());
            if (!string.IsNullOrEmpty(userTotalPoint))
            {
                userPoint.Text = userTotalPoint;
            }
            else
            {
                userPoint.Text = "0";
            }
        }

        private void note_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new PopUp());
        }

    }
}