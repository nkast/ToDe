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

    internal abstract class Konfigurace
    {
        public float SekundMeziVystrely { get; set; }
        public float RychlostRotace { get; set; }
        public float SilaStrely { get; set; }
        public float DosahStrelby { get; set; } // Poloměr rádiusu kruhu dostřelu
        public float Cena { get; set; }
        public float VychoziCenaDemolice { get; set; }
    }

    internal class KonfiguraceVezKulomet : Konfigurace
    {
        public static KonfiguraceVezKulomet VychoziParametry { get; private set; } =
            new KonfiguraceVezKulomet()
            {
                DosahStrelby = Zdroje.VelikostDlazdice * 2.1f,
                RychlostRotace = 90,
                SekundMeziVystrely = 0.5f,
                SilaStrely = 0.05f,
                Cena = 100,
                VychoziCenaDemolice = 50,
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

    internal class KonfiguraceVezRaketa : Konfigurace
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
                VychoziCenaDemolice = 75,
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
}