using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ToDe
{
    internal class Navigace
    {
        public static void SpustitHru(int cisloLevelu, Page stranka)
        {
            TDGame.SouborLevelu = "";
            OvladacHry.NastavCisloLevelu(1);
            SpustitHru(stranka);
        }

        public static void SpustitHru(string soubor, Page stranka)
        {
            TDGame.SouborLevelu = soubor;
            OvladacHry.NastavCisloLevelu(-1);
            SpustitHru(stranka);
        }

        private static void SpustitHru(Page stranka)
        {
            OvladacHry.NastavHru(Nastaveni.Aktualni.NastavHru());
            if (Device.RuntimePlatform == Device.Android)
                ((App)App.Current).SpustPrepnoutNaHru();
            else
                stranka.Navigation.PushAsync(new HraPage());
        }

    }
}
