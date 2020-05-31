using ARGuide.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ARGuide
{
    public class GetUserSerialNumber
    {
        AppValue app = new AppValue();
        public async System.Threading.Tasks.Task<string> getUserSerialNumber(string emailStr)
        {
            string serialNumber = "";
            using (var client = new HttpClient())
            {
                var postData = new UserRegister
                {
                    email = emailStr
                };
                var json = JsonConvert.SerializeObject(postData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var uri = app.url + "/AR_admin/getUserSerialNumber";
                var result = await client.PostAsync(uri, content);
                if (result.IsSuccessStatusCode)
                {
                    var resultString = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Result>(resultString);
                    if (response != null && response.result != null && response.result != "" && response.result == "0")
                    {
                        serialNumber = response.message;
                    }
                }
            }
            return serialNumber;
        }
    }
}
