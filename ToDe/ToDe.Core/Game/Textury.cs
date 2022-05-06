using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    public enum ZakladniDlazdice
    {
        Dira = 0,
        Mapa_Hlina = 1,
        Mapa_Kameni = 2,
        Mapa_Pisek = 3,
        Mapa_Trava = 4,
        Nabidka_Kos = 5,
        Nabidka_Pauza = 6,
        Nabidka_Pauza_Konec = 7,
        Ohen_Kulomet = 8,
        Plocha_Prekazka_6 = 9,
        Plocha_Prekazka_8 = 10,
        Plocha_Prekazka_7 = 11,
        Plocha_Prekazka_5 = 12,
        Plocha_Prekazka_1 = 13,
        Plocha_Prekazka_2 = 14,
        Plocha_Prekazka_4 = 15,
        Plocha_Prekazka_3 = 16,
        Raketa_1 = 17,
        Raketa_2 = 18,
        Tank_Poustni_Spodek = 19,
        Tank_Poustni_Vrsek = 20,
        Tank_Spodek = 21,
        Tank_Vrsek = 22,
        Vez_Elektrika_1 = 23,
        Vez_Elektrika_2 = 24,
        Vez_Kulomet_1 = 25,
        Vez_Kulomet_2 = 26,
        Vez_Raketa_1_Stred = 27,
        Vez_Raketa_1_Vrsek = 28,
        Vez_Raketa_2_Stred = 29,
        Vez_Raketa_2_Vrsek = 30,
        Vez_Raketa_3_Stred = 31,
        Vez_Raketa_3_Vrsek = 32,
        Vez_Zakladna_1 = 33,
        Vez_Zakladna_2 = 34,
        Vez_Zakladna_3 = 35,
        Vez_Zakladna_4 = 36,
        Vojak_Parasutista = 37,
        Vojak_Robot = 38,
        Vojak_Ufon = 39,
        Vojak_Vojak = 40,
    }


    internal class Obsah
    {
        public Textura Zakladni { get; internal set; } = new Textura()
        {
            VelikostDlazdice = new Point(128),
            Okraj = 1,
            Sloupcu = 7,
            Radku = 6,
        };
        public Textura Exploze { get; internal set; } = new Textura()
        {
            VelikostDlazdice = new Point(256),
            Okraj = 0,
            Sloupcu = 8,
            Radku = 6,
        };

        public Textura Ukazatel { get; internal set; } = new Textura()
        {
            VelikostDlazdice = new Point(128, 13),
            Okraj = 1,
            Sloupcu = 1,
            Radku = 2,
        };
        public SpriteFont Pismo { get; internal set; }
        public Zvuk ZvukRaketaStart { get; internal set; }
        public Zvuk ZvukRaketaDopad { get; internal set; }
        public Zvuk ZvukKulomet { get; internal set; }
        public Zvuk ZvukKonecVyhra { get; internal set; }
        public Zvuk ZvukKonecProhra { get; internal set; }
    }

    internal class Textura
    {
        public Point VelikostDlazdice { get; set; }
        public ushort Okraj { get; set; }
        public ushort Sloupcu { get; set; }
        public ushort Radku { get; set; }
        public Texture2D Grafika { get; internal set; }

        public Point SouradniceDlazdice(ZakladniDlazdice zd)
            => new Point((int)zd % Sloupcu, (int)zd / Sloupcu);
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

    internal enum GrafikaDlazdice
    {
        Trava,
        Hlina,
        Kameni,
        Pisek,
    }
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


}
