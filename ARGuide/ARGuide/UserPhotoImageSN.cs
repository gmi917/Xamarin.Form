using ARGuide.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ARGuide
{
    public class UserPhotoImageSN
    {
        AppValue app = new AppValue();
        public async Task<string> getUserPhotoImageSN()
        {
            string sn = "-1";            
                using (var client = new HttpClient())
                {
                    var uri = app.url + "/AR_admin/getUserPhotoImageSN";
                    var resultPoint = await client.GetAsync(uri);
                    if (resultPoint.IsSuccessStatusCode)
                    {
                        string content = await resultPoint.Content.ReadAsStringAsync();
                        //handling the answer  
                        var getSeqData = JsonConvert.DeserializeObject<Result>(content);
                        if (getSeqData != null && getSeqData.result == "0")
                        {
                            sn = getSeqData.message;
                        }                                          
                }
            }
            return sn;
        }
    }
}
