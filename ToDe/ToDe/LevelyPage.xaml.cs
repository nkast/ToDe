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

            XDocument doc = new XDocument(
                new XElement("level",
                    new XElement("finance", 
                        new XAttribute("vychozi", 1000),
                        new XAttribute("prirustek", 10)
                    ),
                     new XElement("mapa",
                        new XAttribute("texturaPlochy", "Trava"),
                        new XAttribute("texturaCesty", "Hlina"),
                        "..............." + Environment.NewLine +
                        "..............." + Environment.NewLine +
                        "..............." + Environment.NewLine +
                        "..............." + Environment.NewLine +
                        "S*************E" + Environment.NewLine +
                        "..............." + Environment.NewLine +
                        "..............." + Environment.NewLine +
                        "..............." + Environment.NewLine +
                        "..............."
                    ),
                     new XElement("vlny",
                        new XElement("vlna",
                            new XElement("jednotka",
                                new XAttribute("typ", "Vojak"),
                                new XAttribute("pocet", 5),
                                new XAttribute("rozestup", 2),
                                new XAttribute("zdravi", 1),
                                new XAttribute("rychlost", 0.5),
                                new XAttribute("sila", 0.1),
                                new XAttribute("uzdravovani", 0.01),
                                new XAttribute("odstup", 0)
                            )
                        )
                     ),
                     new XElement("veze"),
                     new XElement("prekazky")
                )
            );
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