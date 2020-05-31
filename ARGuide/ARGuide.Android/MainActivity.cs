using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Acr.UserDialogs;
using Plugin.Permissions;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Droid;

namespace ARGuide.Droid
{
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
                           Categories = new[]
                           {
                            Android.Content.Intent.CategoryDefault,
                            Android.Content.Intent.CategoryBrowsable
                           },
                           DataScheme = "http",
                           DataHost = "email")]
    [Activity(Label = "ARGuide", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            // 初始化camera Current Activity Plugin
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            //初始化Acr.UserDialogs
            UserDialogs.Init(this);
            //初始化popup
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //初始化 circle image
            ImageCircleRenderer.Init();
            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            base.OnStart();
            var id = "no id";
            if (Intent?.Data != null)
            {
                var alUrl = Intent.Data;
                id = alUrl.GetQueryParameter("id");
                MessagingCenter.Send<Object, string>(this, "emailId", id);
            }
        }

        //request camera permission
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            //用於Android 6.0 之後的版本，彈出用戶請求對話框，獲取相應權限
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}