using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
namespace ARGuide
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();                        
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            //browser call ChangePWD page(url link http://email?id=test@test.com)
            MessagingCenter.Subscribe<Object, string>(this, "emailId", (send, data) =>
            {                                              
                Navigation.PushAsync(new ChangePWD(data));                               
            });
        }

        ~MainPage()
        {           
            MessagingCenter.Unsubscribe<object>(this, "emailId");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            for (int i = 0; i < 11; i ++)
            {
                Device.BeginInvokeOnMainThread(() =>
                {                    
                    timer.Text = i.ToString();
                });
                
                if (i == 10)
                {
                    if (Xamarin.Forms.Application.Current.Properties.ContainsKey("email") != false)
                    {
                        string email = Xamarin.Forms.Application.Current.Properties["email"].ToString();                        
                        if (!string.IsNullOrEmpty(email))
                        {                           
                            await Navigation.PushAsync(new MyProfile(),true);
                            Navigation.RemovePage(this);
                        }
                        else
                        {
                            await Navigation.PushAsync(new Register(),true);
                            Navigation.RemovePage(this);
                        }
                    }
                    else
                    {
                        await Navigation.PushAsync(new Register(),true);
                        Navigation.RemovePage(this);
                    }                                  
                }
                else
                {
                    await Task.Delay(50);
                }
            }
        }
    }
   
}
