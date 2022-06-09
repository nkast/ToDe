using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToDe
{
    public partial class MainPage : ContentPage
    {
        public string TodeImportedFileName { get; set; }
        public OpenImportedFileDelegate OpenImportedFile { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (OpenImportedFile != null)
            {
                string importovanySoubor = String.Empty;
                try
                {
                    using (var stream = await OpenImportedFile())
                        importovanySoubor = Soubory.ImportLevelu(TodeImportedFileName, stream);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Import levelu se nezdařil", 
                        String.Format("Importovaný soubor '{0}' se nepodařilo načíst {1}{1}{2}", 
                            TodeImportedFileName, Environment.NewLine, ex.Message), "OK");
                    return;
                }
                OpenImportedFile = null;
                if (!String.IsNullOrEmpty(importovanySoubor))
                    await Navigation.PushAsync(new LevelPage(Path.GetFileNameWithoutExtension(importovanySoubor)));
                //Navigace.SpustitHru(importovanySoubor, this);
            }
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
