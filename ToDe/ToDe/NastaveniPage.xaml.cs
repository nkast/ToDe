using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NastaveniPage : ContentPage
    {
        public NastaveniPage()
        {
            InitializeComponent();

            tvNastaveni.BindingContext = Nastaveni.Aktualni;
        }

     
    }
}