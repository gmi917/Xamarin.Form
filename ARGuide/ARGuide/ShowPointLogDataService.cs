using ARGuide.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ARGuide
{
    public class ShowPointLogDataService
    {        
        public ObservableCollection<MyItemListData> MyItemList { get; set; } = new ObservableCollection<MyItemListData>();
        public int TotlaCount = 0;
        AppValue app = new AppValue();

        public ShowPointLogDataService()
        {
            if (NetworkCheck.IsInternet())
            {
                using (WebClient webClient = new WebClient())
                {                    
                    Uri address = new Uri(app.url + "/AR_admin/getUserPointLog");
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    UserRegister data = new UserRegister() { email = Application.Current.Properties["email"].ToString() };
                    string json = JsonConvert.SerializeObject(data);
                    string result = webClient.UploadString(address, "POST", json);                 
                    var response = JsonConvert.DeserializeObject<List<PointLog>>(result);
                    if(response!=null)
                    {                 
                        if(response.Count > 0)
                        {
                            TotlaCount = response.Count;
                            for (int i = 0; i < response.Count; i++)
                            {
                                var MyItemListData = new MyItemListData();
                                MyItemListData.createDate = response[i].createDate;
                                MyItemListData.stageName = response[i].stageName;
                                MyItemListData.point = response[i].point;
                                if (i % 2 == 0)
                                {
                                    MyItemListData.CellBackgroundColor = Color.FromHex("#e8e8e8");
                                }
                                else
                                {
                                    MyItemListData.CellBackgroundColor = Color.FromHex("#f6f6f6");
                                }
                                MyItemList.Add(MyItemListData);
                            }
                        }                                                                                                                     
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert("訊息", app.errorMessage, "OK");
                        });
                    }                                       
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "OK");
                });
            }
        }

        public async Task<List<MyItemListData>> GetItemsAsync(int pageIndex, int pageSize)
        {
            await Task.Delay(2000);

            return MyItemList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public class MyItemListData
        {            
            public string createDate { get; set; }
            public string stageName { get; set; }
            public string point { get; set; }
            public Xamarin.Forms.Color CellBackgroundColor { get; set; }
        }
    }
}
