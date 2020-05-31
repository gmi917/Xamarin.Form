using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARGuide;
using ARGuide.iOS;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace ARGuide.iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        private CoreAnimation.CALayer _line;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            _line = null;

            if (Control == null || e.NewElement == null)
                return;

            Control.BorderStyle = UITextBorderStyle.None;

            _line = new CALayer
            {
                BorderColor = UIColor.FromRGB(255, 255, 255).CGColor,
                BackgroundColor = UIColor.FromRGB(174, 174, 174).CGColor,
                Frame = new CGRect(0, Frame.Height / 2, Frame.Width * 2, 1f)
            };

            Control.Layer.AddSublayer(_line);
        }
        //protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        //{
        //    base.OnElementChanged(e);

        //    if (Control != null)
        //    {
        //        //Control.BorderStyle = UITextBorderStyle.None;
        //        //Below line is useful to give border color 
        //        Control.TintColor = UIColor.Red;
        //        //Control.Layer.CornerRadius = 10;
        //        Control.TextColor = UIColor.White;
        //    }
        //}
    }
}