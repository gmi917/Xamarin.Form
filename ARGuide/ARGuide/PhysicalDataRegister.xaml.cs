using ARGuide.Model;
using ARGuide.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ARGuide
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhysicalDataRegister : ContentPage
    {
        PhysicalDataViewModel _context;
        public PhysicalDataRegister()
        {
            InitializeComponent();            
            Title = "建立一個新帳戶";
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#703ce3");
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
            Xamarin.Forms.NavigationPage.SetHasBackButton(this, false);
            _context = new ViewModels.PhysicalDataViewModel();
            BindingContext = _context;
            this.sex.Text = "1";
        }

        private void Editor_Focused(object sender, FocusEventArgs e)
        {
            startDatePicker.IsEnabled = true;
            //startDatePicker.IsVisible = true;
            Device.BeginInvokeOnMainThread(() => {
                startDatePicker.Focus();
            });
            
            //startDatePicker.Focus();
        }

        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            birthday.Text = startDatePicker.Date.ToString("yyyy/MM/dd");
            //birthday.Text = e.NewDate.ToString("yyyy/MM/dd");

        }

        private async void PhysicalDataRegister_Tapped(object sender, EventArgs e)
        {
            var isValid = _context.Validate();
            if (isValid)
            {
                await Navigation.PushAsync(new MemberProfileRegister(new UserRegister()
                {
                    sex = sex.Text,
                    birthday = this.birthday.Text,
                    tall = this.tall.Text,
                    weight = this.weight.Text,
                    age = this.age.Text
                }));
            }
            else
            {
                await DisplayAlert("訊息", "有必填欄位未輸入請重新輸入!", "OK");
            }
           
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

        private void Birthday_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(birthday.Text))
            {
                birthdayLabel.Text = "";
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

        private void womanDisable_Tapped(object sender, EventArgs e)
        {
            this.sex.Text = "0";
            womanEnable.IsVisible = true;
            manDisable.IsVisible = true;
            womanDisable.IsVisible = false;
            manEnable.IsVisible = false;
        }
    }
}