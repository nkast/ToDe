using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToDe;
using ToDe.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HraPage), typeof(HraPageRenderer))]
namespace ToDe.Droid
{
    public class HraPageRenderer : PageRenderer
    {
        public HraPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            Intent intent = new Intent(this.Context, typeof(HraActivity));
            this.Context.StartActivity(intent);
        }
    }

    [Activity(
        Label = "ToDe",
        AlwaysRetainTaskState = false,
        ScreenOrientation = ScreenOrientation.FullSensor,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.Navigation
        )]
    public class HraActivity : AndroidGameActivity
    {
        TDGame hra;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            hra = OvladacHry.SpustitHru(); // new TDGame();
            hra.BackButtonPressed += Game_BackButtonPressed;
            var view = hra.Services.GetService(typeof(Android.Views.View)) as Android.Views.View;
            SetContentView(view);
            hra.Run();
        }

        private void Game_BackButtonPressed(object sender, EventArgs e)
        {
            Finish();
        }

        //public override void OnBackPressed()
        //{
        //    Finish();
        //    base.OnBackPressed();
        //}

        public override void Finish()
        {
            hra.BackButtonPressed -= Game_BackButtonPressed;
            OvladacHry.VypnoutHru();
            hra = null;
            base.Finish();
            //((App)App.Current).SpustPrepnoutNaXF(false);
        }
    }

}