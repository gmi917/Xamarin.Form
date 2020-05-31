using ARGuide.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace ARGuide
{
    public class CleanUserTotalPoint
    {
        AppValue app = new AppValue();
        public async System.Threading.Tasks.Task<bool> cleanUserTotalPoint(string emailStr)
        {
            bool result = false;
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
                    var uri = app.url + "/AR_admin/UserCleanTotalPoint";
                    var postResult = await client.PostAsync(uri, content);
                    if (postResult.IsSuccessStatusCode)
                    {
                        var resultString = await postResult.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<Result>(resultString);
                        if (response != null && response.result != null && response.result != "" && response.result == "0")
                        {
                            result = true;
                        }
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "OK");
            }
            return result;
        }
    }
}
