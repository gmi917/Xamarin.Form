using ARGuide.Model;
using ARGuide.ViewModels;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;

namespace ARGuide
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditProfile : ContentPage
	{
        AppValue app = new AppValue();
        UserPhotoImageSN sn = new UserPhotoImageSN();
        EditProfileViewModel _context;
        string userPhotoImageSN = "", emailStr="", originalSN="";
        bool snResult = false;
        public EditProfile ()
		{
			InitializeComponent ();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasBackButton(this, true);
            Title = "編輯個人資料";
            _context = new ViewModels.EditProfileViewModel();
            BindingContext = _context;
            emailStr = Xamarin.Forms.Application.Current.Properties["email"].ToString();            
            if (!string.IsNullOrEmpty(emailStr))
            {
                getUserData(emailStr);
            }
        }

        public async void getUserData(string emailStr)
        {
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new UserRegister
                    {
                        email= emailStr
                    };
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    //  send a POST request                  
                    var uri = app.url + "/AR_admin/getUserData";
                    var result = await client.PostAsync(uri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        var getUserData = JsonConvert.DeserializeObject<List<UserRegister>>(resultString);
                        if (getUserData != null && getUserData.Count>0)
                        {
                            foreach (var userData in getUserData)
                            {
                                if (!string.IsNullOrEmpty(userData.userPhotoImage))
                                {
                                    circleImage.IsVisible = true;
                                    circleImage.Source = new UriImageSource { CachingEnabled = false, Uri = new Uri(app.url + "/AR_admin/userImage/" + userData.userPhotoImage) };
                                }
                                else
                                {
                                    noImage.IsVisible = true;
                                    circleImage.IsVisible = false;
                                }

                                userName.Text = userData.userName;
                                if (!string.IsNullOrEmpty(userData.sex.ToString()) && userData.sex.Equals("1"))
                                {
                                    this.sex.Text = "1";
                                    manEnable.IsVisible = true;
                                    womanDisable.IsVisible = true;
                                    manDisable.IsVisible = false;
                                    womanEnable.IsVisible = false;                                    
                                }
                                else
                                {
                                    this.sex.Text = "0";
                                    womanEnable.IsVisible = true;
                                    manDisable.IsVisible = true;
                                    manEnable.IsVisible = false;                                                                       
                                    womanDisable.IsVisible = false;
                                }
                                birthday.Text = userData.birthday;
                                age.Text = userData.age;
                                tall.Text = userData.tall;
                                weight.Text = userData.weight;
                                if (!string.IsNullOrEmpty(userData.serialNumber))
                                {
                                    serialNumber.Text = userData.serialNumber;
                                    originalSN = userData.serialNumber;
                                }
                                else
                                {
                                    serialNumber.Text = "";                                    
                                }
                                if (!string.IsNullOrEmpty(userData.deviceNumber))
                                {
                                    deviceText.Text = userData.deviceNumber;
                                }
                                else
                                {
                                    deviceText.Text = "";
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("訊息", "取得使用者資料失敗!", "OK");
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

        private void Birthday_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(birthday.Text))
            {
                birthdayLabel.Text = "";
            }
        }

        private void Editor_Focused(object sender, FocusEventArgs e)
        {
            editStartDatePicker.IsEnabled = true;
                
            Device.BeginInvokeOnMainThread(() => {
                    editStartDatePicker.Focus();
            });                     
        }

        private void editStartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            birthday.Text = editStartDatePicker.Date.ToString("yyyy/MM/dd");
        }

        private async void save_Clicked(object sender, EventArgs e)
        {
            string email = Xamarin.Forms.Application.Current.Properties["email"].ToString();            
            if (string.IsNullOrEmpty(serialNumber.Text) && string.IsNullOrEmpty(deviceText.Text))
            {
                await DisplayAlert("訊息", "請輸入激活碼或者裝置號碼!", "OK");
                getUserData(email);
                return;
            }else if (string.IsNullOrEmpty(serialNumber.Text) && !string.IsNullOrEmpty(deviceText.Text))
            {
                snResult = true;
            }else if (!string.IsNullOrEmpty(serialNumber.Text) && string.IsNullOrEmpty(deviceText.Text))
            {
                CheckSN(serialNumber.Text);
            }
            else if (!string.IsNullOrEmpty(serialNumber.Text) && !string.IsNullOrEmpty(deviceText.Text))
            {
                CheckSN(serialNumber.Text);
            }
            if (snResult)
            {
                var isValid = _context.Validate();
                if (isValid)
                {
                    GetUserSerialNumber getSN = new GetUserSerialNumber();
                    string userSerialNumber = await getSN.getUserSerialNumber(email);
                    if (!string.IsNullOrEmpty(userSerialNumber) && serialNumber.Text != userSerialNumber)
                    {
                        CleanUserTotalPoint cleanPoint = new CleanUserTotalPoint();
                        bool answer = await DisplayAlert("訊息", "注意!!!若修改激活碼則上一場的點數將會全被歸零", "OK", "取消");
                        if (answer == true)
                        {
                            bool cleanResult = await cleanPoint.cleanUserTotalPoint(email);
                            if (cleanResult != true)
                            {
                                await DisplayAlert("訊息", app.errorMessage, "OK");
                                return;
                            }
                        }
                        else
                        {
                            getUserData(email);
                            return;
                        }
                    }

                    if (NetworkCheck.IsInternet())
                    {
                        using (var client = new HttpClient())
                        {
                            var postData = new UserRegister
                            {
                                email = Xamarin.Forms.Application.Current.Properties["email"].ToString(),
                                userName = this.userName.Text,
                                birthday = this.birthday.Text,
                                sex = this.sex.Text,
                                tall = this.tall.Text,
                                weight = this.weight.Text,
                                age = this.age.Text,
                                serialNumber = serialNumber.Text,
                                deviceNumber = deviceText.Text,
                                userPhotoImage = userPhotoImageSN
                            };
                            var json = JsonConvert.SerializeObject(postData);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            //  send a POST request                  
                            var uri = app.url + "/AR_admin/updateUserData";
                            var result = await client.PostAsync(uri, content);
                            if (result.IsSuccessStatusCode)
                            {
                                var resultString = await result.Content.ReadAsStringAsync();
                                var responseData = JsonConvert.DeserializeObject<Result>(resultString);
                                if (responseData != null && responseData.result != "")
                                {
                                    if (responseData.result == "0")
                                    {
                                        await DisplayAlert("訊息", "更新使用者資料成功!", "OK");
                                    }
                                    else if (responseData.result == "1")
                                    {
                                        await DisplayAlert("訊息", app.errorMessage, "OK");
                                    }
                                    else if (responseData.result == "2")
                                    {
                                        await DisplayAlert("訊息", "更新資料失敗!帳號不存在", "OK");
                                    }
                                    else if (responseData.result == "3")
                                    {
                                        await DisplayAlert("訊息", "更新資料失敗!使用者資料為空值", "OK");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("訊息", "更新資料失敗!請稍候再試", "OK");
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
                else
                {
                    await DisplayAlert("訊息", "有必填欄位未輸入或格式錯誤!", "OK");
                }
            }
            else
            {
                await DisplayAlert("訊息", "激活碼有誤請重新輸入!", "OK");
            }
        }

        //choice man
        private void manDisable_Tapped(object sender, EventArgs e)
        {
            this.sex.Text = "1";
            manEnable.IsVisible = true;
            womanDisable.IsVisible = true;
            manDisable.IsVisible = false;
            womanEnable.IsVisible = false;
        }

        //choice woman
        private void womanDisable_Tapped(object sender, EventArgs e)
        {
            this.sex.Text = "0";
            womanEnable.IsVisible = true;
            manDisable.IsVisible = true;
            womanDisable.IsVisible = false;
            manEnable.IsVisible = false;
        }       

        private void userName_Unfocused(object sender, FocusEventArgs e)
        {
            _context.userName.Validate();
        }
      
        private void birthday_Unfocused(object sender, FocusEventArgs e)
        {
            _context.birthday.Validate();
        }

        private void tall_Unfocused(object sender, FocusEventArgs e)
        {
            _context.tall.Validate();
        }

        private void weight_Unfocused(object sender, FocusEventArgs e)
        {
            _context.weight.Validate();
        }

        private void age_Unfocused(object sender, FocusEventArgs e)
        {
            _context.age.Validate();
        }

        private void serialNumber_Unfocused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(serialNumber.Text))
            {
                CheckSN(serialNumber.Text);
            }
            else
            {
                if (!string.IsNullOrEmpty(originalSN))
                {
                    CheckSN(serialNumber.Text);                    
                }
                else
                {
                    //empty sn
                    SNError.Text = "";
                    snResult = true;
                }                
            }                                    
        }
        public void CheckSN(string sn)
        {            
            bool checkResult = false;
            if(!string.IsNullOrEmpty(sn))
            {
                checkResult = webClient(sn);
                if (checkResult)
                {
                    SNError.Text = "";
                    snResult = true;
                }
                else
                {
                    string validateStr = ValidateUserSN(sn);
                    if (!string.IsNullOrEmpty(validateStr))
                    {
                        if (validateStr == "0")
                        {
                            SNError.Text = "";
                            snResult = true;
                        }
                        else if (validateStr == "1")
                        {
                            SNError.Text = "激活碼已經被使用過";
                            snResult = false;
                        }
                        else if (validateStr == "2")
                        {
                            SNError.Text = "激活碼不存在";
                            snResult = false;
                        }
                    }
                }
            }
            else
            {
                //user clean sn
                SNError.Text = "";
                snResult = true;
            }                    
        }
        public string ValidateUserSN(String sn)
        {
            string returnResult = "";
            try
            {
                if (!string.IsNullOrEmpty(sn))
                {
                    if (NetworkCheck.IsInternet())
                    {
                        var webClient = new WebClient();
                        Uri uri = new Uri(app.url + "/AR_admin/checkSN/" + sn);
                        var result = webClient.DownloadString(uri);
                        var data = JsonConvert.DeserializeObject<Result>(result);
                        if (data != null && data.result != "")
                        {
                            returnResult = data.result;
                        }
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "OK");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("訊息", app.errorMessage, "OK");
                });
            }
            return returnResult;
        }

        public bool webClient(string inputSerialNumber)
        {
            bool original = false;
            string serverSerialNumber = "";
            string emailStr = "";
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
            {
                emailStr = Xamarin.Forms.Application.Current.Properties["email"].ToString();
            }
            if (NetworkCheck.IsInternet())
            {
                using (WebClient client = new WebClient())
                {
                    Uri address = new Uri(app.url + "/AR_admin/getUserSerialNumber");
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    UserRegister data = new UserRegister() { email = emailStr };
                    string json = JsonConvert.SerializeObject(data);
                    string result = client.UploadString(address, "POST", json);
                    var post = JsonConvert.DeserializeObject<Result>(result);
                    if (post != null && post.result != null && post.result != "" && post.result == "0")
                    {
                        serverSerialNumber = post.message;
                        if (!string.IsNullOrEmpty(serverSerialNumber) && !string.IsNullOrEmpty(inputSerialNumber) && serverSerialNumber.Equals(inputSerialNumber))
                        {
                            original = true;
                        }
                        else
                        {
                            original = false;
                        }
                    }
                    else
                    {
                        original = false;
                    }
                }
            }
            else
            {
                DisplayAlert("訊息", app.networkMessage, "OK");
            }

            return original;
        }

        private async void editPhoto_Tapped(object sender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                var action = await DisplayActionSheet("更換頭像", "取消", null, "從相冊選擇", "拍照");
                switch (action)
                {
                    case "從相冊選擇":
                        await SelectFromImageLibrary();
                        break;
                    case "拍照":
                        await TakePhotoFromCamera();
                        break;
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "OK");
            }
        }

        async Task SelectFromImageLibrary()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("訊息", "無法打開相冊;設備不支援", "OK");
                return;
            }

            //Check Permission
            var photosStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
            if (photosStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Photos });
                photosStatus = results[Permission.Photos];
            }

            if (photosStatus == PermissionStatus.Granted)
            {
                var file = await CrossMedia.Current.PickPhotoAsync(
                    new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Small,
                        CompressionQuality = 92,
                    });

                if (file == null)
                    return;

                if (NetworkCheck.IsInternet())
                {
                    userPhotoImageSN =await sn.getUserPhotoImageSN();
                    if(!string.IsNullOrEmpty(userPhotoImageSN) && userPhotoImageSN != "-1")
                    {
                        postImage(file, userPhotoImageSN);
                        //show Image                    
                        circleImage.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            file.Dispose();
                            return stream;
                        });
                    }
                    else
                    {
                        await DisplayAlert("訊息", "取得序號資料失敗;請稍候再更改照片!", "OK");
                    }                    
                }
                else
                {
                    await DisplayAlert("訊息", app.networkMessage, "OK");
                }                                       
            }
            else
            {
                await DisplayAlert("訊息", "沒有權限開啟相冊", "OK");

                //iOS客戶端時打開設置界面
                if (Device.RuntimePlatform == Device.iOS)
                {
                    CrossPermissions.Current.OpenAppSettings();
                }
            }
        }

        async Task TakePhotoFromCamera()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("訊息", "無法開啟鏡頭;設備不支援", "OK");
                return;
            }

            //Check camera and storage Permission
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    Name = "photo.jpg"
                });

                if (file == null)
                    return;

                if (NetworkCheck.IsInternet())
                {
                    userPhotoImageSN = await sn.getUserPhotoImageSN();
                    if(!string.IsNullOrEmpty(userPhotoImageSN)  && userPhotoImageSN != "-1")
                    {
                        postImage(file, userPhotoImageSN);
                        // show ImageSource                                     
                        circleImage.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            file.Dispose();
                            return stream;
                        });
                    }
                    else
                    {
                        await DisplayAlert("訊息", "取得序號資料失敗;請稍候再更改照片!", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("訊息", app.networkMessage, "OK");
                }                                        
            }
            else
            {
                await DisplayAlert("訊息", "沒有權限開啟鏡頭或進行檔案儲存", "OK");

                //iOS客戶端時打開設置界面
                if (Device.RuntimePlatform == Device.iOS)
                {
                    CrossPermissions.Current.OpenAppSettings();
                }
            }
        }

        private async void postImage(MediaFile userImageFile, string sn)
        {
            UserRegister postData = null;
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(sn))
                    {
                        //將剛剛拍照的檔案，上傳到網路伺服器上                    
                        using (var content = new MultipartFormDataContent())
                        {
                            // get path
                            var path = userImageFile.Path;
                            // get filename
                            var filename = Path.GetFileName(path);
                            //get file extension
                            int position = filename.IndexOf(".");
                                                        
                            // read content
                            using (var fs = userImageFile.GetStream())
                            {
                                var streamContent = new StreamContent(fs);
                                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                                streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + Path.GetFileName(path) + "\"");
                                content.Add(streamContent, "file", filename);
                                if (!string.IsNullOrEmpty(emailStr) && !string.IsNullOrEmpty(sn))
                                {
                                    postData = new UserRegister
                                    {
                                        email = emailStr,
                                        userPhotoImage = sn
                                    };
                                }
                                if (postData != null && !string.IsNullOrEmpty(postData.email)
                                    && !string.IsNullOrEmpty(postData.userPhotoImage))
                                {
                                    content.Add(new StringContent(emailStr), "email");
                                    content.Add(new StringContent(sn), "userPhotoImage");
                                    //content.Add(new StringContent(postData.ToString()), "user");
                                }
                                else
                                {
                                    await DisplayAlert("訊息", "上傳圖檔失敗;無使用者資料請重新登入!", "OK");
                                }
                                
                                // 上傳到遠端伺服器上
                                var response = await client.PostAsync(app.url + "/AR_admin/uploadfile", content);
                                if (response.IsSuccessStatusCode)
                                {
                                    var result = response.Content.ReadAsStringAsync().Result;                                    
                                    string responseContent = await response.Content.ReadAsStringAsync();
                                    //handling the answer  
                                    var getResponse = JsonConvert.DeserializeObject<Result>(responseContent);
                                    if (getResponse != null && getResponse.result == "0")
                                    {
                                        await DisplayAlert("訊息", "上傳圖檔成功", "OK");
                                    }
                                    else
                                    {
                                        await DisplayAlert("訊息", "上傳圖檔失敗;請稍候再更改照片", "OK");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("訊息", "上傳圖檔失敗;請稍候再更改照片", "OK");
                                }
                            }
                        }
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