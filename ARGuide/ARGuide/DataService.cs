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
    public class DataService
    {
        public ObservableCollection<MyItemListData> MyItemList { get; set; }= new ObservableCollection<MyItemListData>();
        public int TotlaCount = 0;
        AppValue app = new AppValue();
        //public ObservableCollection<someDataClass> MyItemList { get; set; }
        public DataService()
        {
            if (NetworkCheck.IsInternet())
            {
                if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
                {                    
                    using (WebClient webClient = new WebClient())
                    {
                        Uri address = new Uri(app.url + "/AR_admin/UserfindallPrize");
                        webClient.Encoding = Encoding.UTF8;
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                        UserRegister data = new UserRegister() { email = Application.Current.Properties["email"].ToString()};
                        string json = JsonConvert.SerializeObject(data);
                        string result = webClient.UploadString(address, "POST", json);
                        var posts = JsonConvert.DeserializeObject<List<PrizeContent>>(result);
                        if (posts != null)
                        {
                            //handling the answer  
                            TotlaCount = posts.Count;
                            if (posts.Count > 0)
                            {
                                
                                //MyItemList = new ObservableCollection<MyItemListData>();
                                foreach (var postData in posts)
                                {
                                    MyItemList.Add(new MyItemListData { id = postData.id, prizeName = postData.prizeName, point = postData.point, image = app.url + postData.image });
                                }
                            }
                        }                      
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
            public string id { get; set; }
            public string categoryID { get; set; }
            public string prizeName { get; set; }
            public string point { get; set; }
            public string image { get; set; }
        }

    }
}
