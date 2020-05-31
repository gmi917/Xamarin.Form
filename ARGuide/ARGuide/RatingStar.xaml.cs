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
	public partial class RatingStar : ContentPage
	{
        string prizeId = "";
        AppValue app = new AppValue();
        public RatingStar (PrizeDetailItem arg)
		{
			InitializeComponent ();
            Title = "我要評論";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#ff8400");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            prizeId = arg.id;
            Uri uriImage = new Uri(app.url + arg.image);
            prizeImage.Source = ImageSource.FromUri(uriImage);
            prizeName.Text = arg.prizeName;
            today.Text = DateTime.Now.ToString("yyyy/MM/dd");           
        }

        private async void ratingStar_Tapped(object sender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new RatingStarPrize
                    {
                        email = Xamarin.Forms.Application.Current.Properties["email"].ToString(),
                        prizeID = prizeId,
                        ratingStar = this.ratingStar.Text,
                        comment = this.editFeedback.Text
                    };
                    // create the request content and define Json  
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    //  send a POST request                  
                    var uri = app.url + "/AR_admin/UserRatingStarPrize";
                    var result = await client.PostAsync(uri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        await DisplayAlert("訊息", "感謝您寶貴的建議!", "OK");                        
                        await Navigation.PushAsync(new PrizeListView(),true);
                        Navigation.RemovePage(this);
                    }
                    else
                    {
                        await DisplayAlert("訊息", "填寫評論失敗", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "OK");
            }
        }
    }
}