using ARGuide.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static ARGuide.DataService;

namespace ARGuide
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrizeListView : ContentPage
    {
        string userTotalPoint = "0";
        public PrizeListView()
        {
            InitializeComponent();
            Title = "積分兌換平台";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#ff8400");

            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
            {
                showTotalPoint();                                
            }            
        }

        public async void showTotalPoint()
        {
            GetUserTotalPoint getUserPoint = new GetUserTotalPoint();
            userTotalPoint= await getUserPoint.getUserTotalPoint(Xamarin.Forms.Application.Current.Properties["email"].ToString());
            if (!string.IsNullOrEmpty(userTotalPoint) && int.Parse(userTotalPoint)>=0)
            {
                userPoint.Text = userTotalPoint;
            }
            else if(!string.IsNullOrEmpty(userTotalPoint) && userTotalPoint =="-1")
            {
                await DisplayAlert("訊息", "取得使用者點數資料失敗!", "OK");
            }
            else
            {
                userPoint.Text = "0";
            }            
        }

        public async void MainListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Selected = e.Item as MyItemListData;
            var prizeItem = new PrizeDetailItem
            {
                id = Selected.id
            };
            await Navigation.PushAsync(new PrizeDetail(prizeItem),true);            
        }
        private void goHome_Clicked(object sender, EventArgs e)
        {

        }
    }
}