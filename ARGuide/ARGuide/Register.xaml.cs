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
	public partial class Register : ContentPage
	{
		public Register ()
		{
			InitializeComponent ();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);              
        }

        private async void emailRegister_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PhysicalDataRegister(),true);              
        }

        private async void accountLogin_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login(),true);           
        }
    }
}