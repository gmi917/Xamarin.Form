using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace ARGuide
{
    public class NetworkCheck
    {
        public static bool IsInternet()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                // Connection to internet is available
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
