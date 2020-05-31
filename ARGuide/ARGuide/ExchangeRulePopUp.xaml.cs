using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ARGuide
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PopUp : PopupPage
    {
		public PopUp ()
		{
			InitializeComponent ();
		}

        private async void known_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}