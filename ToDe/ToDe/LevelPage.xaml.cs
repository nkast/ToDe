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
    public partial class LevelPage : ContentPage
    {
        string soubor;
        string Soubor
        {
            get => soubor;
            set
            {
                soubor = value;
                Title = soubor + (Zmena ? "*" : "");
            }
        }

        bool zmena = false;
        bool Zmena
        {
            get => zmena;
            set
            {
                zmena = value;
                Title = soubor + (zmena ? "*" : "");
            }
        }

        public LevelPage(string soubor)
        {
            InitializeComponent();

            Soubor = soubor;
            eLevel.Text = File.ReadAllText(Soubory.CestaSouboruLevelu(soubor));
            Zmena = false;
            eLevel.FontSize = Preferences.Get("LevelEditorFontSize", eLevel.FontSize);
        }

        protected override bool OnBackButtonPressed()
        {
            if (Zmena)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await DisplayAlert("Zavřít",
                            "Level není uložen, přejete si přesto odejít", "Odejít", "Zrušit"))
                        await Navigation.PopAsync();
                });
                return true;
            }
                return base.OnBackButtonPressed();
        }

        private void bUlozit_Clicked(object sender, EventArgs e)
        {
            Uloz();
            Zmena = false;
        }

        private async void bUlozitJako_Clicked(object sender, EventArgs e)
        {
            string nazev = await Soubory.ZeptejSeNaNovyNazevSoboru(this, "Uložit jako", Soubor);
            if (!String.IsNullOrEmpty(nazev))
            {
                Uloz(nazev);
                Soubor = nazev;
                Zmena = false;
            }
        }

        private async void bPrejmenovat_Clicked(object sender, EventArgs e)
        {
            string nazev = await Soubory.ZeptejSeNaNovyNazevSoboru(this, "Přejmenovat", Soubor);
            if (!String.IsNullOrEmpty(nazev))
            {
                File.Move(Soubory.CestaSouboruLevelu(Soubor), Soubory.CestaSouboruLevelu(nazev));
                Soubor = nazev;
            }
        }

        private async void bVymazat_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Vymazat",
                       String.Format("Skutečně si přejete level '{0}' vymazat?", soubor), "Vymazat", "Zrušit"))
            {
                File.Delete(Soubory.CestaSouboruLevelu(soubor));
                await Navigation.PopAsync();
            }
        }

        private void eLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            Zmena = true;
        }

        private async void bFormatovat_Clicked(object sender, EventArgs e)
        {
            try
            {
                Uloz();
                var doc = XDocument.Parse(eLevel.Text);
                eLevel.Text = doc.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Chyba", 
                    "XML dokument obsahuje chyby:" + Environment.NewLine + ex.Message, "OK");
            }
        }

        private async void bVelikostPisma_Clicked(object sender, EventArgs e)
        {
            string velikost = await DisplayActionSheet("Velikost písma", "Zrušit", null, 
                nameof(NamedSize.Micro),
                nameof(NamedSize.Small),
                nameof(NamedSize.Medium),
                nameof(NamedSize.Large));
            if (Enum.TryParse<NamedSize>(velikost, out NamedSize vyber))
            {
                double velikostPisma = Device.GetNamedSize(vyber, eLevel);
                eLevel.FontSize = velikostPisma;
                Preferences.Set("LevelEditorFontSize", velikostPisma);
            }
        }

        private void bHrat_Clicked(object sender, EventArgs e)
        {
            Uloz();
            TDGame.SouborLevelu = Soubory.CestaSouboruLevelu(Soubor);
            Navigation.PushAsync(new HraPage());
        }

        private void bSdilet_Clicked(object sender, EventArgs e)
        {
            Uloz();
            Share.RequestAsync(new ShareFileRequest("Sdílet level z ToDe", new ShareFile(Soubory.CestaSouboruLevelu(Soubor))));
        }

        void Uloz(string sobor = null)
        {
            File.WriteAllText(Soubory.CestaSouboruLevelu(soubor ?? Soubor), eLevel.Text);
        }
    }
}