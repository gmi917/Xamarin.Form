using ARGuide.Model;
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
	public partial class Login : ContentPage
	{
        AppValue app = new AppValue();
        GetUserTotalPoint getUserPoint = new GetUserTotalPoint();
        public Login ()
		{
			InitializeComponent ();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

        }

        private void RsetPassword_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Forget(),true);
        }

        private async void login_Tapped(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(email.Text) && !string.IsNullOrEmpty(password.Text))
            {
                if (NetworkCheck.IsInternet())
                {
                    using (var client = new HttpClient())
                    {
                        var postData = new UserRegister
                        {
                            email = email.Text,
                            userPWD = password.Text
                        };
                        // create the request content and define Json  
                        var json = JsonConvert.SerializeObject(postData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        //  send a POST request                  
                        var uri = app.url + "/AR_admin/userlogin";
                        var result = await client.PostAsync(uri, content);

                        if (result.IsSuccessStatusCode)
                        {
                            var resultString = await result.Content.ReadAsStringAsync();
                            var post = JsonConvert.DeserializeObject<Result>(resultString);
                            if (post != null && post.result != null && post.result != "" && post.result == "0")
                            {
                                if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
                                {
                                    Xamarin.Forms.Application.Current.Properties["email"] = email.Text;                                    
                                }
                                else
                                {
                                    Xamarin.Forms.Application.Current.Properties.Add("email", email.Text);
                                }
                                await Xamarin.Forms.Application.Current.SavePropertiesAsync();                                                        
                                
                                //write user login log
                                var postLogData = new UserLog
                                {
                                    email = email.Text
                                };
                                var jsonLog = JsonConvert.SerializeObject(postLogData);
                                var contentLog = new StringContent(jsonLog, Encoding.UTF8, "application/json");

                                //  send a POST request                  
                                var uriLog = app.url + "/AR_admin/UserLoginLog";
                                var resultLog = await client.PostAsync(uriLog, contentLog);

                                //導到選擇模式畫面                               
                                await Navigation.PushAsync(new MyProfile(),true);
                                Navigation.RemovePage(this);                                                                
                            }
                            else
                            {
                                await DisplayAlert("訊息", "登入失敗!", "OK");
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
                await DisplayAlert("訊息", "請輸入Email和密碼!", "OK");
            }
        }

        private async void back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register(),true);
        }
    }
}