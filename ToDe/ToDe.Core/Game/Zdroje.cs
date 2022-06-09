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
    internal class Zdroje
    {
        public static Zdroje Aktualni { get; private set; }

        public static Obsah Obsah { get; internal set; }

        public static NastaveniHry Nastaveni { get; internal set; } = new NastaveniHry();
        

        public static int VelikostDlazdice { get => (int)Obsah.Zakladni.VelikostDlazdice.X; }
        
        public static int CisloLevelu = 1;

        public Level Level { get; private set; }


        private Zdroje() { }

        public static Zdroje NactiLevel(ref int cisloMapy)
        {
            string soubor = string.Format("Content/Levels/Level{0}.xml", cisloMapy);
            return NactiLevel(soubor, cisloMapy);
        }

        public static Zdroje NactiLevel(string soubor, int cisloMapy = -1)
        {
            // Načtení streamu
            //string soubor = string.Format("Content/Levels/Level{0}.xml", cisloMapy);

            var zdroje = new Zdroje();
            zdroje.Level = new Level();
            zdroje.Level.Cislo = cisloMapy;
            Aktualni = zdroje;

            XDocument doc;
            try
            {
                if (cisloMapy >= 0)
                    using (Stream fileStream = TitleContainer.OpenStream(soubor))
                        doc = XDocument.Load(fileStream);
                else
                    doc = XDocument.Load(soubor);
                zdroje.Level = Level.Nacti(doc.Root);
            }
            catch (Exception ex)
            {
                if (cisloMapy >= 0)
                {
                    cisloMapy = 1;
                    zdroje = NactiLevel(ref cisloMapy);
                }
                else
                    throw;
            }

            return zdroje;
        }
      
    }
}
