using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToDe
{
    public enum TypVeze
    {
        Kulomet,
        Raketa,
    }

    internal abstract class KonfiguraceVeze
    {
        public float SekundMeziVystrely { get; set; }
        public float RychlostRotace { get; set; }
        public float SilaStrely { get; set; }
        public float DosahStrelby { get; set; } // Poloměr rádiusu kruhu dostřelu
        public float Cena { get; set; }
        public float VychoziPrijemZDemolice { get; set; }
    }

    internal class KonfiguraceVezKulomet : KonfiguraceVeze
    {
        public static KonfiguraceVezKulomet VychoziParametry { get; private set; } =
            new KonfiguraceVezKulomet()
            {
                DosahStrelby = Zdroje.VelikostDlazdice * 2.1f,
                RychlostRotace = 90,
                SekundMeziVystrely = 0.5f,
                SilaStrely = 0.05f,
                Cena = 100,
                VychoziPrijemZDemolice = 50,
            };

        static Dictionary<ushort, KonfiguraceVezKulomet> Vlasntosti = new Dictionary<ushort, KonfiguraceVezKulomet>()
        {
            { 1, VychoziParametry }
        };

        public static KonfiguraceVezKulomet ParametryVeze(ushort uroven)
        {
            if (Vlasntosti.ContainsKey(uroven))
                return Vlasntosti[uroven];
            return Vlasntosti.Values.Last();
        }
    }

    internal class KonfiguraceVezRaketa : KonfiguraceVeze
    {
        public float DosahExploze { get; set; }
        public float RychlostRakety { get; set; }

        public static KonfiguraceVezRaketa VychoziParametry { get; private set; } =
            new KonfiguraceVezRaketa()
            {
                DosahStrelby = Zdroje.VelikostDlazdice * 4.1f,
                RychlostRotace = 45,
                SekundMeziVystrely = 2f,
                SilaStrely = 0.34f,
                Cena = 150,
                DosahExploze = Zdroje.VelikostDlazdice * 1.5f,
                RychlostRakety = Zdroje.VelikostDlazdice * 2.0f,
                VychoziPrijemZDemolice = 75,
            };

        static Dictionary<ushort, KonfiguraceVezRaketa> Vlasntosti = new Dictionary<ushort, KonfiguraceVezRaketa>()
        {
            { 1, VychoziParametry }
        };

        public static KonfiguraceVezRaketa ParametryVeze(ushort uroven)
        {
            if (Vlasntosti.ContainsKey(uroven))
                return Vlasntosti[uroven];
            return Vlasntosti.Values.Last();
        }
    }

    internal static class KonfiguracePrekazek
    {
        public static Dictionary<char, float> Cenik { get; private set; } = 
            new Dictionary<char, float> 
            {
                { '1', -10 },
                { '2', -15 },
                { '3', -20 },
                { '4', -25 },
                { '5', -30 },
                { '6', -40 },
                { '7', -50 },
                { '8', -60 },
            };


        public static ZakladniDlazdice DlazdicePrekazky(char znakPrekazky) 
        {
            if (Enum.TryParse<ZakladniDlazdice>(nameof(ZakladniDlazdice.Plocha_Prekazka_1).Replace('1', znakPrekazky),
                out ZakladniDlazdice dlazdice))
                return dlazdice;
            return ZakladniDlazdice.Plocha_Prekazka_1;
        }

        public static bool ZnakJePrekazka(char znak) => "12345678".Contains(znak);

    }
}