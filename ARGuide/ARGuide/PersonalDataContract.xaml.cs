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
	public partial class PersonalDataContract : ContentPage
	{
		public PersonalDataContract ()
		{
			InitializeComponent ();
            Title = "蒐集個人資料告知事項";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#2f2f2f");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;

        }
    }
}