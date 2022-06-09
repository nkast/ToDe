using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ToDe.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            //LoadApplication(new ToDe.App());


            todeFileForImport = (App.Current as App)?.TodeFileForImport;
            ToDe.App xfApp;
            if (todeFileForImport == null)
                xfApp = new ToDe.App();
            else
                xfApp = new ToDe.App(todeFileForImport.Name, OpenImportedFile);

            LoadApplication(xfApp);
        }


        StorageFile todeFileForImport;

        public async Task<Stream> OpenImportedFile()
        {
            return await todeFileForImport?.OpenStreamForReadAsync();
        }
    }
}
