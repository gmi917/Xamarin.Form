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
	public partial class PrizeDetail : ContentPage
	{
        AppValue app = new AppValue();
        GetUserTotalPoint getUserPoint = new GetUserTotalPoint();
        string userTotalPoint = "0",prizePoint = "", prizeId="", imgUrl="";
        //int amount = 0;
        public PrizeDetail (PrizeDetailItem arg)
		{
			InitializeComponent ();
            Title = "點數兌換";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#ff8400");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            if (NetworkCheck.IsInternet())
            {                
                Show(app.url + "/AR_admin/UsergetPrizeDetailbyId/" + arg.id);
            }
            else
            {
                DisplayAlert("訊息", app.networkMessage, "OK");
            }
        }

        private void add_Tapped(object sender, EventArgs e)
        {
            prizeAmount.Text= Convert.ToString(int.Parse(prizeAmount.Text) + 1);
            totalPrizePoint.Text = Convert.ToString(int.Parse(prizeAmount.Text)* int.Parse(prizePoint));
        }

        private void less_Tapped(object sender, EventArgs e)
        {
            if (int.Parse(prizeAmount.Text) <= 0)
            {
                prizeAmount.Text = "0";
                totalPrizePoint.Text = "0";
            }
            else
            {
                prizeAmount.Text = Convert.ToString(int.Parse(prizeAmount.Text) - 1);
                totalPrizePoint.Text = Convert.ToString(int.Parse(prizeAmount.Text) * int.Parse(prizePoint));
            }            
        }

        async void exchange_Tapped(object sender, EventArgs e)
        {            
            if (int.Parse(prizeAmount.Text)>0)
            {                
                if (int.Parse(userTotalPoint) >= int.Parse(totalPrizePoint.Text))
                {
                    var prizeItem = new PrizeDetailItem
                    {
                        id = prizeId,
                        prizeName = PrizeName.Text,
                        amount = prizeAmount.Text,
                        point = prizePoint,
                        image = imgUrl
                    };
                    await Navigation.PushAsync(new PrizeExchange(prizeItem),true);                    
                }
                else
                {
                    await DisplayAlert("訊息", "您的點數不夠兌換該產品", "OK");
                }
            }
            else
            {
                await DisplayAlert("訊息", "請點選新增產品數量", "OK");
            }           
        }

        private async void Show(string url)
        {
            userTotalPoint = await getUserPoint.getUserTotalPoint(Xamarin.Forms.Application.Current.Properties["email"].ToString());            
            if(userTotalPoint!=null && userTotalPoint!="" && int.Parse(userTotalPoint) >= 0)
            {
                var jsonData = await GetJsonDataAsync(url);
                if(jsonData!=null && jsonData != "")
                {
                    var posts = JsonConvert.DeserializeObject<List<PrizeDetailItem>>(jsonData);
                    if (posts.Count > 0)
                    {
                        foreach (var postData in posts)
                        {
                            prizeId = postData.id;
                            imgUrl = postData.image;                            
                            Uri uriImage = new Uri(app.url + postData.image);
                            prizeImg.Source = ImageSource.FromUri(uriImage);                            
                            userPoint.Text = userTotalPoint;
                            totalPrizePoint.Text = Convert.ToString(int.Parse(prizeAmount.Text)*int.Parse(postData.point));
                            PrizeName.Text = postData.prizeName;
                            PrizeDescription.Text = postData.prizeDescription;
                            prizePoint = postData.point;
                            PrizePoint.Text = postData.point;
                            PrizeCategoryName.Text = postData.categoryName;
                        }
                    }
                }
                else
                {
                    await DisplayAlert("訊息", "查無產品詳細資料!", "OK");
                }                
            }
            else
            {
                await DisplayAlert("訊息", "取得使用者點數資料失敗!", "OK");
            }           
        }

        public async Task<string> GetJsonDataAsync(string url)
        {
            string jsonContent = null;
            using (var client = new HttpClient())
            {
                var uri = url;
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    jsonContent = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    await DisplayAlert("訊息", "取得產品資料失敗!", "OK");
                }
            }
            return jsonContent;
        }
    }
}