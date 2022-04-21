using Microsoft.Xna.Framework;
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
        Ground,
        Road,
        //GR_DR,
        //GR_D,
        //GR_DL,
        //GR_R,
        //GR_L,
        //GR_TR,
        //GR_T,
        //GR_TL,
    }

    public enum KamJit
    {
        Nahoru = 270,
        Doprava = 0,
        Dolu = 90,
        Doleva = 180,
    }

    internal class Textury
    {
        public Texture2D Zakladni { get; internal set; }
        public Texture2D Exploze { get; internal set; }
        public SpriteFont Pismo { get; internal set; }
    }

    internal class Zdroje // TODO: přejmenovat
    {
        public static Zdroje Aktualni { get; private set; }

        public static Textury Textury { get; internal set; }

        public const int VelikostDlazdice = 128;

        public Level Level { get; private set; }


        private Zdroje() { }

        public static Zdroje NactiMapu(int cisloMapy)
        {
            // Načtení streamu
            string soubor = string.Format("Content/Levels/Level{0}.xml", cisloMapy);
            var mapa = new Zdroje();
            mapa.Level = new Level();
            mapa.Level.Cislo = cisloMapy;
            Aktualni = mapa;

            XDocument doc;
            using (Stream fileStream = TitleContainer.OpenStream(soubor))
                    doc = XDocument.Load(fileStream);
            mapa.Level = Level.Nacti(doc.Root);

            return mapa;
        }

      
    }
}
