using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ToDe
{
    public class HraPage : ContentPage
    {
        public HraPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            OvladacHry.VypnoutHru();
            return base.OnBackButtonPressed();
        }
    }
}