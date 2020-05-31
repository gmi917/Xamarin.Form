using ARGuide.Model;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
	public partial class PrizeExchangePopup : PopupPage
    {
        AppValue app = new AppValue();
        string prizeId = "", exchangePoint = "", exchangeAmount = "", imageSource = "", prizeName = "", prizeTotalPoint="";
        public PrizeExchangePopup (PrizeDetailItem arg)
		{
			InitializeComponent ();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            prizeId = arg.id;
            popPrizeName.Text = arg.prizeName;
            prizeName = arg.prizeName;
            prizeTotalPoint = Convert.ToString(int.Parse(arg.point) * int.Parse(arg.amount));
            exchangePoint = arg.point;
            popPrizePoint.Text = prizeTotalPoint;
            exchangeAmount = arg.amount;
            popPrizeAmount.Text= arg.amount;
            Uri uriImage = new Uri(app.url + arg.image);
            //PrizeImg.Source = ImageSource.FromUri(uriImage);
            imageSource = arg.image;
        }

        private async void Cancel_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void send_Tapped(object sender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new MemberNumber
                    {
                        prizeId = prizeId,
                        memberNumber = EnterMemberNumber.Text
                    };
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var checkUri = app.url + "/AR_admin/checkMemberNumber";
                    var result = await client.PostAsync(checkUri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        var post = JsonConvert.DeserializeObject<Result>(resultString);

                        if (post.result != null && post.result != "" && post.result == "0")
                        {
                            var postOrderData = new PrizeOrder
                            {
                                prizeId = prizeId,
                                email = Xamarin.Forms.Application.Current.Properties["email"].ToString(),
                                amount = exchangeAmount,
                                prizePoint = prizeTotalPoint
                            };
                            var jsonOrder = JsonConvert.SerializeObject(postOrderData);
                            var contentOrder = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

                            var uri = app.url + "/AR_admin/UserExchangePrize";
                            var resultOrder = await client.PostAsync(uri, contentOrder);
                            if (resultOrder.IsSuccessStatusCode)
                            {
                                var resultOrderString = await result.Content.ReadAsStringAsync();
                                var postOrder = JsonConvert.DeserializeObject<Result>(resultString);

                                if (postOrder.result != null && postOrder.result != "" && postOrder.result == "0")
                                {
                                    await DisplayAlert("訊息", "兌換成功", "Ok");
                                    var prizeItem = new PrizeDetailItem
                                    {
                                        id = prizeId,
                                        prizeName = prizeName,
                                        amount = exchangeAmount,
                                        point = exchangePoint,
                                        image = imageSource
                                    };
                                    await Navigation.PushAsync(new RatingStar(prizeItem),true);
                                    await PopupNavigation.Instance.PopAsync();                                    
                                }
                                else
                                {
                                    await DisplayAlert("訊息", "兌換失敗!請稍候再試", "Ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("訊息", app.errorMessage, "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("訊息", "輸入的店家序號錯誤!", "Ok");
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

    }
}