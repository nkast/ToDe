using MonoGame.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDe;
using ToDe.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(HraPage), typeof(HraPageRenderer))]
namespace ToDe.UWP
{
    public class HraPageRenderer : PageRenderer
    {
        Windows.UI.Xaml.Controls.Page page;
        SwapChainPanel scpMain;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            page = new Windows.UI.Xaml.Controls.Page();
            scpMain = new SwapChainPanel();
            page.Content = scpMain;
            var game = XamlGame<TDGame>.Create("", Window.Current.CoreWindow, scpMain);

            this.Children.Add(page);
        }
    }
}
