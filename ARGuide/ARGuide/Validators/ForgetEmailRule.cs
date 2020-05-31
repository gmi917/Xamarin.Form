using ARGuide.Model;
using Newtonsoft.Json;
using Plugin.ValidationRules.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ARGuide.Validators
{
    public class ForgetEmailRule<T> : IValidationRule<T>
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
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(str);
                    result = match.Success;
                    if (result)
                    {
                        checkResult = webClient(str);
                        if (checkResult)
                        {
                            result = true;
                        }
                        else
                        {
                            ValidationMessage = "這個email不存在;請重新輸入";
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        public bool webClient(string emailStr)
        {
            bool isExist = false;
            using (WebClient client = new WebClient())
            {
                Uri address = new Uri(app.url + "/AR_admin/hasUser");
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                UserRegister data = new UserRegister() { email = emailStr };
                string json = JsonConvert.SerializeObject(data);
                string result = client.UploadString(address, "POST", json);
                var post = JsonConvert.DeserializeObject<Result>(result);
                if (post != null && post.result != null && post.result != "" && post.result == "0")
                {
                    isExist = true;
                }
                else
                {
                    isExist = false;
                }
            }
            return isExist;
        }
    }
}
