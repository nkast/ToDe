﻿using Microsoft.Xna.Framework;
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

        public static int VelikostDlazdice { get => (int)Obsah.Zakladni.VelikostDlazdice.X; }
        
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
