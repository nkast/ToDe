using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LevelyPage : ContentPage
    {
        public LevelyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NactiSeznamSouboru();
        }

        void NactiSeznamSouboru()
        {
            lvSeznamLevelu.ItemsSource = Soubory.SeznamLevelu();
        }

        void OtevriLevel(string nazevSouboru)
        {
            Navigation.PushAsync(new LevelPage(nazevSouboru));
        }

        private async void bPridat_Clicked(object sender, EventArgs e)
        {
            string nazev = await Soubory.ZeptejSeNaNovyNazevSoboru(this, "Nový level");
            if (String.IsNullOrEmpty(nazev))
                return;         

            var doc = OvladacHry.VychoziLevel();
            doc.Save(Soubory.CestaSouboruLevelu(nazev));

            OtevriLevel(nazev);
        }

        private void lvSeznamLevelu_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            string soubor = e.Item as string;
            if (String.IsNullOrEmpty(soubor)) return;
            OtevriLevel(soubor);
        }
    }
}