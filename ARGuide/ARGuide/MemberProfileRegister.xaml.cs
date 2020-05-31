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
using ARGuide.Validators;
using System.Net;

namespace ARGuide
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MemberProfileRegister : ContentPage
    {
        MemberViewModel _context;
        AppValue app = new AppValue();
        UserPhotoImageSN sn = new UserPhotoImageSN();
        String sex = "", birthday = "", tall = "", weight = "", age = "", userPhotoImageSN = "", extension="";
        bool snResult=false;
        public MemberProfileRegister(UserRegister arg)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Title = "註冊";            
            _context = new ViewModels.MemberViewModel();
            BindingContext = _context;
            this.sex = arg.sex;
            this.birthday = arg.birthday;
            this.tall = arg.tall;
            this.weight = arg.weight;
            this.age = arg.age;
        }

        async Task SelectFromImageLibrary()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("訊息", "無法打開相簿;設備不支援", "OK");
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
                    userPhotoImageSN = await sn.getUserPhotoImageSN();//取得照片檔案序號
                    if (!string.IsNullOrEmpty(userPhotoImageSN) && userPhotoImageSN != "-1")
                    {
                        postImage(file, userPhotoImageSN);
                        //show Image
                        circleImage.IsVisible = true;
                        img.IsVisible = false;
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
                await DisplayAlert("訊息", "沒有權限開啟相簿", "OK");

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
                    userPhotoImageSN = await sn.getUserPhotoImageSN();//取得照片檔案序號
                    if (!string.IsNullOrEmpty(userPhotoImageSN) && userPhotoImageSN != "-1")
                    {                        
                        postImage(file, userPhotoImageSN);
                        // show ImageSource
                        circleImage.IsVisible = true;
                        img.IsVisible = false;
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

        private void contract_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Contract(),true);
        }

        private async void photoImageRegister_Tapped(object sender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                var action = await DisplayActionSheet("從哪裏新增照片？", "取消", null, "拍照", "選擇照片");
                switch (action)
                {
                    case "選擇照片":
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

        private async void circleImage_Tapped(object sender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                var action = await DisplayActionSheet("從哪裏新增照片？", "取消", null, "拍照", "選擇照片");
                switch (action)
                {
                    case "選擇照片":
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

        private async void add_Tapped(object sender, EventArgs e)
        {
            string serialNumberText = "", deviceNumberText = "";
            if (checkBox.IsChecked == true)
            {
                var isValid = _context.Validate();
                if (isValid)
                {
                    if (password.Text.Equals(confirmPassword.Text))
                    {
                        if (!string.IsNullOrEmpty(snText.Text) || !string.IsNullOrEmpty(deviceText.Text))
                        {
                            if (!string.IsNullOrEmpty(snText.Text))
                            {
                                ValidateUserSN(snText.Text);                                
                            }
                            if (!string.IsNullOrEmpty(this.deviceText.Text) && string.IsNullOrEmpty(this.snText.Text))
                            {
                                snResult = true;
                            }

                            if(snResult)
                            {
                                if (NetworkCheck.IsInternet())
                                {
                                    if (!string.IsNullOrEmpty(this.snText.Text))
                                    {
                                        serialNumberText = this.snText.Text;
                                    }
                                    if (!string.IsNullOrEmpty(this.deviceText.Text))
                                    {
                                        deviceNumberText = this.deviceText.Text;                                        
                                    }
                                    
                                    using (var client = new HttpClient())
                                    {
                                        if (!string.IsNullOrEmpty(userPhotoImageSN))
                                        {
                                            userPhotoImageSN = userPhotoImageSN + extension;
                                        }
                                        var postData = new UserRegister
                                        {
                                            sex = this.sex,
                                            birthday = this.birthday,
                                            tall = this.tall,
                                            weight = this.weight,
                                            age = this.age,
                                            userName = this.userName.Text,
                                            email = this.email.Text,
                                            userPWD = this.password.Text,
                                            serialNumber = serialNumberText,
                                            deviceNumber = deviceNumberText,
                                            userPhotoImage = userPhotoImageSN
                                        };
                                        //create the request content and define Json
                                        var json = JsonConvert.SerializeObject(postData);
                                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                                        //  send a POST request                  
                                        var uri = app.url + "/AR_admin/addUser";
                                        var result = await client.PostAsync(uri, content);
                                        if (result.IsSuccessStatusCode)
                                        {
                                            var resultString = await result.Content.ReadAsStringAsync();
                                            var post = JsonConvert.DeserializeObject<Result>(resultString);
                                            if (post != null && post.result != null && post.result != "" && post.result == "0")
                                            {
                                                if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
                                                {
                                                    Xamarin.Forms.Application.Current.Properties["email"] = email.Text;
                                                }
                                                else
                                                {
                                                    Xamarin.Forms.Application.Current.Properties.Add("email", email.Text);
                                                }
                                                await Xamarin.Forms.Application.Current.SavePropertiesAsync();
                                                await DisplayAlert("訊息", "註冊帳號成功!", "OK");

                                                await Navigation.PushAsync(new MyProfile(), true);
                                                Navigation.RemovePage(this);
                                            }
                                            else
                                            {
                                                await DisplayAlert("訊息", "註冊帳號失敗!請稍候再試", "OK");
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
                                await DisplayAlert("訊息", "激活碼有誤請重新輸入!", "OK");
                            }                            
                        }
                        else
                        {
                            await DisplayAlert("訊息", "請輸入激活碼或者裝置號碼!", "OK");
                        }                        
                    }
                    else
                    {
                        await DisplayAlert("訊息", "密碼不一致請重新輸入", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("訊息", "有必填欄位未輸入或格式錯誤!", "OK");
                }                   
            }
            else
            {
                await DisplayAlert("訊息", "未勾選同意合約", "OK");
            }            
        }

        private void personalDataContract_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PersonalDataContract(), true);
        }

        private async void postImage(MediaFile userImageFile,string sn)
        {
            UserRegister postData = null;
            if (NetworkCheck.IsInternet())
            {                
                using (var client = new HttpClient())
                {                                    
                    if (!string.IsNullOrEmpty(sn))
                    {
                        //upload image to server                     
                        using (var content = new MultipartFormDataContent())
                        {                           
                            // get path
                            var path = userImageFile.Path;
                            // get filename
                            var filename = Path.GetFileName(path);                            
                            //get file extension
                            int position = filename.IndexOf(".");
                            if (position > 0)
                            {
                                extension = filename.Substring(position);                                
                            }
                                
                            // read content
                            using (var fs = userImageFile.GetStream())
                            {
                                var streamContent = new StreamContent(fs);
                                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                                streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + Path.GetFileName(path) + "\"");
                                content.Add(streamContent, "file", filename);
                                if (!string.IsNullOrEmpty(sn))
                                {
                                    postData = new UserRegister
                                    { 
                                        userPhotoImage = sn
                                    };
                                }
                                if (postData != null && !string.IsNullOrEmpty(postData.userPhotoImage))
                                {
                                    content.Add(new StringContent(""), "email");
                                    content.Add(new StringContent(sn), "userPhotoImage");                                    
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
                    else
                    {
                        await DisplayAlert("訊息", "取得序號資料失敗;請稍候再更改照片!", "OK");
                    }
                }               
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "OK");
            }
            
        }

        private void userName_Unfocused(object sender, FocusEventArgs e)
        {
            _context.userName.Validate();
        }
        

        private void email_Unfocused(object sender, FocusEventArgs e)
        {
            _context.email.Validate();
        }

        private void password_Unfocused(object sender, FocusEventArgs e)
        {
            _context.password.Validate();
        }

        private void confirmPassword_Unfocused(object sender, FocusEventArgs e)
        {
            _context.confirmPassword.Validate();
        }

        private void serialNumber_Unfocused(object sender, FocusEventArgs e)
        {           
            if (!string.IsNullOrEmpty(snText.Text))
            {
                //_context.serialNumber.Validate();
                ValidateUserSN(snText.Text);
                
                //serialNumber.Text = "測試";
            }            
        }

        public void ValidateUserSN(String sn)
        {            
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
                        if (data != null && data.result!="")
                        {                            
                            if (data.result == "0")
                            {
                                serialNumber.Text = "";
                                snResult = true;
                            }
                            else if (data.result == "1")
                            {
                                serialNumber.Text = "激活碼已經被使用過";
                                snResult = false;
                            }
                            else if (data.result == "2")
                            {
                                serialNumber.Text = "激活碼不存在";
                                snResult = false;
                            }
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
        }
    }
}