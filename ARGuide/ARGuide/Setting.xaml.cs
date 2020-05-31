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
	public partial class Setting : ContentPage
	{
		public Setting ()
		{
			InitializeComponent ();
            Title = "設定";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasBackButton(this, false);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#2f2f2f");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
        }

        private async void editUserData_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfile(),true);
        }

        private async void profile_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Contract(),true);
        }

        private void company_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Company(), true);
        }        

        private async void logout_Tapped(object sender, EventArgs e)
        {
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
            {
                Xamarin.Forms.Application.Current.Properties["email"] = "";
                Xamarin.Forms.Application.Current.Properties.Clear();
                await Xamarin.Forms.Application.Current.SavePropertiesAsync();
                await Navigation.PushAsync(new Register(),true);
                Navigation.RemovePage(this);
            }
        }

        private async void myProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyProfile());
        }

        protected override void OnAppearing()
        {
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#2f2f2f");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            base.OnAppearing();
        }

        private void personalData_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PersonalDataContract(),true);
        }
    }
}