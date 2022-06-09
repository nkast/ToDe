using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToDe
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void bNovaHra_Clicked(object sender, EventArgs e)
        {
            Navigace.SpustitHru(1, this);
        }

        private void bEditorLevelu_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LevelyPage());
        }

        private void bNastaveni_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NastaveniPage());
        }
    }
}
