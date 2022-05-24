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
        public float PrijemZDemolice { get; set; }
        public float PocetStrilen { get; set; }
    }

    internal class KonfiguraceVezKulomet : KonfiguraceVeze
    {
        public static KonfiguraceVezKulomet VychoziParametry { get; private set; } =
            new KonfiguraceVezKulomet()
            {
                DosahStrelby = /*Zdroje.VelikostDlazdice */ 2.1f,
                RychlostRotace = 90,
                SekundMeziVystrely = 0.75f,
                SilaStrely = 0.025f,
                Cena = 100,
                PrijemZDemolice = 50,
                PocetStrilen = 1,
            };

        static Dictionary<ushort, KonfiguraceVezKulomet> Vlasntosti = new Dictionary<ushort, KonfiguraceVezKulomet>()
        {
            { 1, VychoziParametry },
            { 2, new KonfiguraceVezKulomet() {
                DosahStrelby = 2.5f,
                RychlostRotace = 110,
                SekundMeziVystrely = 0.65f,
                SilaStrely = 0.035f,
                Cena = 70,
                PrijemZDemolice = 75, 
                PocetStrilen = 1,
            } },
            { 3, new KonfiguraceVezKulomet() {
                DosahStrelby = 3f,
                RychlostRotace = 130,
                SekundMeziVystrely = 0.55f,
                SilaStrely = 0.045f,
                Cena = 70,
                PrijemZDemolice = 90,
                PocetStrilen = 1,
            } },
        };

        public static KonfiguraceVezKulomet ParametryVeze(ushort uroven)
        {
            if (Vlasntosti.ContainsKey(uroven))
                return Vlasntosti[uroven];
            return null;// Vlasntosti.Values.Last();
        }

        //public ushort MaxUroven => MaxUroven;
        public static ushort MaxUroven => Vlasntosti.Keys.Max();
    }

    internal class KonfiguraceVezRaketa : KonfiguraceVeze
    {
        public float DosahExploze { get; set; }
        public float RychlostRakety { get; set; }

        public static KonfiguraceVezRaketa VychoziParametry { get; private set; } =
            new KonfiguraceVezRaketa()
            {
                DosahStrelby = 4f,
                RychlostRotace = 45,
                SekundMeziVystrely = 2f,
                SilaStrely = 0.34f,
                Cena = 150,
                DosahExploze = 1.5f,
                RychlostRakety =  2.0f,
                PrijemZDemolice = 75,
                PocetStrilen = 1,
            };

        static Dictionary<ushort, KonfiguraceVezRaketa> Vlasntosti = new Dictionary<ushort, KonfiguraceVezRaketa>()
        {
            { 1, VychoziParametry },
            { 2, new KonfiguraceVezRaketa()
            {
                DosahStrelby = VychoziParametry.DosahStrelby * 1.1f,
                RychlostRotace = VychoziParametry.RychlostRotace * 1.1f,
                SekundMeziVystrely = VychoziParametry.SekundMeziVystrely * 0.9f,
                SilaStrely = VychoziParametry.SilaStrely * 1.1f,
                Cena = 100,
                DosahExploze = VychoziParametry.DosahExploze * 1.1f,
                RychlostRakety = VychoziParametry.RychlostRakety * 1.1f,
                PrijemZDemolice = 100,
                PocetStrilen = 1,
            } },
        };

        public static KonfiguraceVezRaketa ParametryVeze(ushort uroven)
        {
            if (Vlasntosti.ContainsKey(uroven))
                return Vlasntosti[uroven];
            return null; // Vlasntosti.Values.Last();
        }
        public static ushort MaxUroven => Vlasntosti.Keys.Max();
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