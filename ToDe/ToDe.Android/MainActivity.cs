using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using Android.Database;
using Android.Provider;

namespace ToDe.Droid
{
    [Activity(Label = "ToDe", 
              Icon = "@mipmap/icon", 
              Theme = "@style/MainTheme", 
              LaunchMode = LaunchMode.SingleInstance, 
              MainLauncher = true, 
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "file",
        DataHost = "*",
        DataMimeType = "*/*",
        DataPathPattern = @".*\\.tode")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataMimeTypes = new[] { "application/x-tode" })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var Intent_DataString = Intent?.DataString;
            OpenImportedFileDelegate GetOpenImportedFile = null;
            string fileName = null;
            if (Intent?.Data != null)
            {
                fileName = SkutecneJmenoSouboru(Intent);
                GetOpenImportedFile = OpenImportedFile;
            }

            // #GM
            App app = null;
            if (GetOpenImportedFile == null)
                app = new App();
            else
                app = new App(fileName, GetOpenImportedFile);
            app.PrepnoutNaHru += App_PrepnoutNaHru;
            LoadApplication(app);
        }

        // #GM
        private void App_PrepnoutNaHru(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(HraActivity));
            StartActivity(intent);
        }

        public async Task<Stream> OpenImportedFile()
        {
            //return await Platforms.StreamToMemoryStream(ContentResolver.OpenInputStream(Intent.Data));
            var ms = new MemoryStream();
            using (var zdroj = ContentResolver.OpenInputStream(Intent.Data))
            {
                await zdroj.CopyToAsync(ms);
                await zdroj.FlushAsync();
                //zdroj.Close();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        // Zjistí název importovaného souboru
        public string SkutecneJmenoSouboru(Intent data)
        {
            // Get the Uri of the selected file
            var uri = data.Data;
            var uriString = uri.ToString();
            var myFile = new Java.IO.File(uriString);
            //var path = myFile.AbsolutePath;
            String displayName = null;

            if (uriString.StartsWith("content://"))
            {
                ICursor cursor = null;
                try
                {
                    cursor = ContentResolver.Query(uri, null, null, null, null);
                    if (cursor != null && cursor.MoveToFirst())
                        displayName = cursor.GetString(cursor.GetColumnIndex(IOpenableColumns.DisplayName));
                }
                finally
                {
                    cursor?.Close();
                }
            }
            else if (uriString.StartsWith("file://"))
                displayName = myFile.Name;
            return displayName;
        }
    }
}