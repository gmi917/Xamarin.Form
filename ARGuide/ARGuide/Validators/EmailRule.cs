using ARGuide.Model;
using Newtonsoft.Json;
using Plugin.ValidationRules.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ARGuide.Validators
{
    public class EmailRule<T> : IValidationRule<T>
    {
        AppValue app = new AppValue();
        public string ValidationMessage { get; set; }
        public bool Check(T value)
        {
            bool result = false;
            bool checkResult = false;
            if (value == null)
            {
                return false;
            }
            else
            {
                var str = value as string;
                if (!string.IsNullOrEmpty(str))
                {
                    //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    //Match match = regex.Match(str);
                    //result= match.Success;

                    if (str.Contains("@"))
                    {                       
                        checkResult = webClient(str);
                        if (checkResult)
                        {
                            result = true;
                        }
                        else
                        {
                            ValidationMessage = "這個email已經有人使用;請試試其他名稱";
                            result = false;
                        }
                    }
                    else
                    {
                        result= false;
                    }
                }
            }
            return result;
        }
       
        public bool webClient(string emailStr)
        {
            bool isDuplicate = false;
            if (NetworkCheck.IsInternet())
            {
                using (WebClient client = new WebClient())
                {
                    Uri address = new Uri(app.url + "/AR_admin/checkUser");
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    UserRegister data = new UserRegister() { email = emailStr };
                    string json = JsonConvert.SerializeObject(data);
                    string result = client.UploadString(address, "POST", json);
                    var post = JsonConvert.DeserializeObject<Result>(result);
                    if (post != null && post.result != null && post.result != "" && post.result == "0")
                    {
                        isDuplicate = true;
                    }
                    else
                    {
                        isDuplicate = false;
                    }
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "OK");
            }
            
            return isDuplicate;
        }
    }
}
