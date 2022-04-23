using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ToDe
{
    internal enum TypDlazdice
    {
        Plocha,
        Cesta,
        //GR_DR,
        //GR_D,
        //GR_DL,
        //GR_R,
        //GR_L,
        //GR_TR,
        //GR_T,
        //GR_TL,
    }

    internal enum GrafikaDlazdice
    {
       Trava,
       Hlina,
       Kameni,
       Pisek,
    }

    public enum KamJit
    {
        Nahoru = 270,
        Doprava = 0,
        Dolu = 90,
        Doleva = 180,
    }

    internal class Zvuk
    {
        public SoundEffect ZvukovyEfekt { get; private set; }
        public ushort PocetSoubeznychPrehrani { get; internal set; }
        public float ChranenaCastZvuku { get; internal set; } // Část zvuku (%) po kterou nesmí začít hrát další zvuk

        List<float> ZacatkyPrehravani;

        public Zvuk(SoundEffect zvukovyEfekt, ushort pocetSoubeznychPrehrani = 3, float chranenaCastZvuku = 1)
        {
            ZvukovyEfekt = zvukovyEfekt;
            ChranenaCastZvuku = chranenaCastZvuku;
            PocetSoubeznychPrehrani = pocetSoubeznychPrehrani;
            ZacatkyPrehravani = new List<float>();
        }

        public void HrajZvuk(float aktualniCasHry)
        {
            ZacatkyPrehravani.RemoveAll(x => x + ZvukovyEfekt.Duration.TotalSeconds * ChranenaCastZvuku < aktualniCasHry);
            if (ZacatkyPrehravani.Count < PocetSoubeznychPrehrani)
            {
                ZvukovyEfekt.Play();
                ZacatkyPrehravani.Add(aktualniCasHry);
            }
        }
    }

    internal class Obsah
    {
        public Texture2D Zakladni { get; internal set; }
        public Texture2D Exploze { get; internal set; }
        public SpriteFont Pismo { get; internal set; }
        public Zvuk ZvukRaketaStart { get; internal set; }
        public Zvuk ZvukRaketaDopad { get; internal set; }
        public Zvuk ZvukKulomet { get; internal set; }
        public Zvuk ZvukKonecVyhra { get; internal set; }
        public Zvuk ZvukKonecProhra { get; internal set; }
    }

    internal class Zdroje 
    {
        public static Zdroje Aktualni { get; private set; }

        public static Obsah Obsah { get; internal set; }

        public const int VelikostDlazdice = 128;
        
        public static int CisloLevelu = 1;

        public Level Level { get; private set; }


        private Zdroje() { }

        public static Zdroje NactiLevel(ref int cisloMapy)
        {
            // Načtení streamu
            string soubor = string.Format("Content/Levels/Level{0}.xml", cisloMapy);

            var mapa = new Zdroje();
            mapa.Level = new Level();
            mapa.Level.Cislo = cisloMapy;
            Aktualni = mapa;

            XDocument doc;
            try
            {
                using (Stream fileStream = TitleContainer.OpenStream(soubor))
                    doc = XDocument.Load(fileStream);
                mapa.Level = Level.Nacti(doc.Root);
            }
            catch (Exception)
            {
                cisloMapy = 1;
                mapa = NactiLevel(ref cisloMapy);
            }

            return mapa;
        }

      
    }
}
