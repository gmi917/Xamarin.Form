using ARGuide.Model;
using ARGuide.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ARGuide
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Forget : ContentPage
	{
        AppValue app = new AppValue();
        ForgetPWDViewModel _context;
        public Forget ()
		{
			InitializeComponent ();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            _context = new ViewModels.ForgetPWDViewModel();
            BindingContext = _context;
        }

        private async void back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }

        private void email_Unfocused(object sender, FocusEventArgs e)
        {
            _context.email.Validate();
        }

        private async void resetPWD_Tapped(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(email.Text))
            {
                if (NetworkCheck.IsInternet())
                {
                    using (var client = new HttpClient())
                    {
                        var postData = new UserRegister
                        {
                            email = email.Text                            
                        };
                        var json = JsonConvert.SerializeObject(postData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var uri = app.url + "/AR_admin/sendMail";
                        var result = await client.PostAsync(uri, content);

                        if (result.IsSuccessStatusCode)
                        {
                            var resultString = await result.Content.ReadAsStringAsync();
                            var post = JsonConvert.DeserializeObject<Result>(resultString);
                            if (post != null && post.result != null && post.result != "" && post.result == "0")
                            {
                                checkMailBox.IsVisible = true;
                                resetBtn.IsVisible = false;
                            }
                            else
                            {
                                await DisplayAlert("訊息", "發送Email失敗!", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("訊息", app.errorMessage, "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("訊息", app.networkMessage, "OK");
                }
            }
            else
            {
                await DisplayAlert("訊息", "請輸入Email!", "OK");
            }
        }
    }
}