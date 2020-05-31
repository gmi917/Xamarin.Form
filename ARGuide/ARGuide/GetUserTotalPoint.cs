using ARGuide.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace ARGuide
{
    public class GetUserTotalPoint
    {
        AppValue app = new AppValue();
        public async System.Threading.Tasks.Task<string> getUserTotalPoint(string emailStr)
        {
            string totalPoint = "-1";
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new UserRegister
                    {
                        email = emailStr
                    };
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var uri = app.url + "/AR_admin/UsergetTotalPoint";
                    var result = await client.PostAsync(uri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<Result>(resultString);
                        if (response != null && response.result != null && response.result != "" && response.result == "0")
                        {
                            totalPoint = response.message;
                        }
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "OK");
            }            
            return totalPoint;
        }
    }
}
