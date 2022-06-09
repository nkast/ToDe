using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDe
{
    internal class Nastaveni : INotifyPropertyChanged
    {
        // Výchozí hodnoty
        private bool prehravatZvuky = true;

        // Aktuální instance nastavení
        static Nastaveni aktualni;
        public static Nastaveni Aktualni
        {
            get
            {
                if (aktualni == null)
                {
                    aktualni = new Nastaveni();
                }
                return aktualni;
            }
        }

        // Konstruktor (načtení hodnot z úložiště)
        private Nastaveni()
        {
            prehravatZvuky = Get(nameof(PrehravatZvuky), prehravatZvuky);
        }

        public NastaveniHry NastavHru()
        {
            return new NastaveniHry()
            {
                PrehravatZvuky = PrehravatZvuky,
            };
        }

        // Vlasntosti nastavení
        public bool PrehravatZvuky { get => prehravatZvuky; set { prehravatZvuky = value; Set(value); OnChanged(); } }

        // Pomocné metody pro přístup k úložišti
        public static void Set(bool value, [CallerMemberName] string key = null) => Preferences.Set(key, value);
        public static void Set(string value, [CallerMemberName] string key = null) => Preferences.Set(key, value);
        public static void Set(int value, [CallerMemberName] string key = null) => Preferences.Set(key, value);
        public static void Set(double value, [CallerMemberName] string key = null) => Preferences.Set(key, value);
        public static void Set(Color value, [CallerMemberName] string key = null) => Preferences.Set(key, value.ToHex());

        public static bool Get(string key, bool defaultValue) => Preferences.Get(key, defaultValue);
        public static string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);
        public static int Get(string key, int defaultValue) => Preferences.Get(key, defaultValue);
        public static double Get(string key, double defaultValue) => Preferences.Get(key, defaultValue);
        public static Color Get(string key, Color defaultValue) => Color.FromHex(Preferences.Get(key, defaultValue.ToHex()));

        // Oznámení změny hodnoty
        public event PropertyChangedEventHandler PropertyChanged;
        void OnChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
