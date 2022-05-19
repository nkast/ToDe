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
        ScreenOrientation = ScreenOrientation.FullUser,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden
        )]
    public class HraActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var game = new TDGame();
            var view = game.Services.GetService(typeof(Android.Views.View)) as Android.Views.View;
            SetContentView(view);
            game.Run();
        }
    }
}