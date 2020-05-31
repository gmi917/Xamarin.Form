using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;
using Xamarin.Forms;

namespace ARGuide.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //初始化popup
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            //初始化 circle image
            ImageCircleRenderer.Init();
            LoadApplication(new App());            
            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var isEmail = url.Host.Contains("email");
            if (isEmail)
            {
                var emailID = url.Query.Split("&").Select(x =>
                {
                    if (x.StartsWith("id"))
                    {
                        return x.Substring(3, x.Length - 3);
                    }
                    else
                    {
                        return "no id";
                    }
                }).FirstOrDefault();

                MessagingCenter.Send<object, string>(this, "emailId", emailID);
                return true;
            }
            return true;
        }
    }
}
