using ARGuide.Model;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
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
	public partial class MyProfile : ContentPage
	{
        AppValue app = new AppValue();        
        UserPhotoImageSN sn = new UserPhotoImageSN();
        public MyProfile ()
		{            
            InitializeComponent();
            Title = "個人檔案";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasBackButton(this, false);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#2f2f2f");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;            
            if (NetworkCheck.IsInternet())
            {
                getUserData();
                setSelect(Xamarin.Forms.Application.Current.Properties["email"].ToString());
            }
            else
            {
                DisplayAlert("訊息", app.networkMessage, "OK");
            }
        }

        private async void setting_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Setting(),true);
        }

        private async void editPhoto_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfile(),true);
        }
       
        public async void getUserData()
        {
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new UserRegister
                    {
                        email = Xamarin.Forms.Application.Current.Properties["email"].ToString()
                    };
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    //  send a POST request                  
                    var uri = app.url + "/AR_admin/getUserData";
                    var result = await client.PostAsync(uri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        var getUserData = JsonConvert.DeserializeObject<List<UserRegister>>(resultString);
                        if (getUserData != null && getUserData.Count > 0)
                        {
                            foreach (var userData in getUserData)
                            {
                                if (!string.IsNullOrEmpty(userData.userPhotoImage))
                                {
                                    circleImage.IsVisible = true;
                                    circleImage.Source = new UriImageSource { CachingEnabled = false, Uri = new Uri(app.url + "/AR_admin/userImage/" + userData.userPhotoImage.Trim()) };
                                }
                                else
                                {
                                    noImage.IsVisible = true;
                                    circleImage.IsVisible=false;
                                }
                               
                                userName.Text = userData.userName;
                                deviceNumber.Text = userData.deviceNumber;
                                if (!string.IsNullOrEmpty(userData.sex.ToString()) && userData.sex=="1")
                                {
                                    man.IsVisible = true;
                                }
                                else
                                {
                                    women.IsVisible = true;
                                }
                                age.Text = userData.age;
                                tall.Text = userData.tall;
                                weight.Text = userData.weight;
                            }
                        }
                        else
                        {
                            await DisplayAlert("訊息", "取得使用者資料失敗!", "OK");
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

        public async void setSelect(string emailStr)
        {
            if (NetworkCheck.IsInternet())
            {
                if (!string.IsNullOrEmpty(emailStr))
                {
                    using (var client = new HttpClient())
                    {
                        var postData = new UserRegister
                        {
                            email = emailStr
                        };
                        var json = JsonConvert.SerializeObject(postData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var uri = app.url + "/AR_admin/getUserSelection";
                        var result = await client.PostAsync(uri, content);
                        if (result.IsSuccessStatusCode)
                        {
                            var resultString = await result.Content.ReadAsStringAsync();
                            var response = JsonConvert.DeserializeObject<Result>(resultString);
                            if (response != null && response.result != null && response.result != "" && response.result == "0")
                            {
                                if (response.message == "DeviceNumber and SerialNumber")
                                {
                                    sport.IsVisible = true;
                                    tour.IsVisible = true;
                                }
                                else if (response.message == "DeviceNumber")
                                {
                                    sport.IsVisible = true;
                                    tour.IsVisible = false;
                                }
                                else if (response.message == "SerialNumber")
                                {
                                    sport.IsVisible = false;
                                    tour.IsVisible = true;
                                }
                                else
                                {
                                    sport.IsVisible = false;
                                    tour.IsVisible = false;
                                }
                            }
                            else
                            {
                                sport.IsVisible = false;
                                tour.IsVisible = false;
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
                    await DisplayAlert("訊息", "查無登入資料;請重新登入!", "OK");
                    await Navigation.PushAsync(new Register(),true);
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "OK");
            }
        }

        private void sport_Tapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new PrizeListView(),true);
        }

        private async void tour_Tapped(object sender, EventArgs e)
        {            
            await Navigation.PushAsync(new PrizeListView(),true);
        }

        //refresh user data
        protected override void OnAppearing() {
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#2f2f2f");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            getUserData();
            base.OnAppearing();
        }
    }
}