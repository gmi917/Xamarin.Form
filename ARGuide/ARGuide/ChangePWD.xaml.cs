using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace ARGuide
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePWD : ContentPage
    {
        AppValue app = new AppValue();
        public ChangePWD(string id)
        {
            InitializeComponent();
            Title = "更改密碼";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#2f2f2f");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            Xamarin.Forms.NavigationPage.SetHasBackButton(this, false);
            //DisplayAlert("message", id, "ok");            
        }

        private void close_Clicked(object sender, EventArgs e)
        {

        }

        private async void changePWD_Tapped(object sender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                if (!string.IsNullOrEmpty(newPWD.Text) && !string.IsNullOrEmpty(confirmPWD.Text))
                {
                    if (newPWD.Text == confirmPWD.Text)
                    {

                    }
                    else
                    {
                        await DisplayAlert("訊息", "密碼不一致;請重新輸入!", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("訊息", "請輸入密碼", "OK");
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "OK");
            }
        }

        private void cHidePWD_Tapped(object sender, EventArgs e)
        {
            confirmPWD.IsPassword = false;
            cHidePWD.IsVisible = false;
            cShowPWD.IsVisible = true;
        }
        private void cShowPWDS_Tapped(object sender, EventArgs e)
        {
            confirmPWD.IsPassword = true;
            cHidePWD.IsVisible = true;
            cShowPWD.IsVisible = false;
        }

        private void nHidePWD_Tapped(object sender, EventArgs e)
        {
            newPWD.IsPassword = false;
            nHidePWD.IsVisible = false;
            nShowPWD.IsVisible=true;
        }

        private void nShowPWD_Tapped(object sender, EventArgs e)
        {
            newPWD.IsPassword = true;
            nHidePWD.IsVisible = true;
            nShowPWD.IsVisible = false;
        }
       
    }
}