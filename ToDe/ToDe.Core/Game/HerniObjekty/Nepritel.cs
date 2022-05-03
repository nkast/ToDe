using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal class Nepritel : HerniObjekt
    {
        public float Zdravi { get; set; } = 1;
        public float MaxZdravi { get; set; } = 1;
        public int IndexPoziceNaTrase { get; set; }
        public Vector2 SouradniceCile { get; set; }
        public float VzdalenostNaCeste { get; private set; }
        public float SilaUtoku { get; private set; }
        public bool DosahlCile { get; private set; } = false;
        public TypNepritele Typ { get; set; }
        public UkazatelZdravi Ukazatel { get; set; }
        public float ProcentoZdravi { get; private set; }
        public float Uzdravovani { get; private set; }

        public Nepritel(LevelVlnaJednotka jednotka)
        {
            Dlazdice = jednotka.Dlazdice(); // new[] { new DlazdiceUrceni(17, 10, 0.1f) };
            UhelKorkceObrazku = 0;

            Typ = jednotka.Typ;
            Zdravi = jednotka.Zdravi;
            MaxZdravi = jednotka.Zdravi;
            Uzdravovani = jednotka.Uzdravovani;
            ProcentoZdravi = 1;
            SilaUtoku = jednotka.Sila;
            RychlostPohybu = Zdroje.VelikostDlazdice * jednotka.Rychlost;

            RychlostRotace = 90 * RychlostPohybu / (Zdroje.VelikostDlazdice / 2); // Za dobu ujití poloviny dlaždice se otočí o 90°
            Pozice = Zdroje.Aktualni.Level.Mapa.PoziceNaTrase(0);
            SouradniceCile = Zdroje.Aktualni.Level.Mapa.PoziceNaTrase(1);
            IndexPoziceNaTrase = 0;
            UhelOtoceni = (int)Zdroje.Aktualni.Level.Mapa.SmerDalsiTrasy(Zdroje.Aktualni.Level.Mapa.TrasaPochodu[0], Zdroje.Aktualni.Level.Mapa.TrasaPochodu[1]);

            Ukazatel = new UkazatelZdravi(this);
        }

        public override void Update(float sekundOdMinule)
        {
            DosahlCile = false;

            if (Smazat) return;

            if (Uzdravovani != 0)
                Zdravi = MathHelper.Clamp(Zdravi + Uzdravovani * sekundOdMinule, 0, MaxZdravi);

            UhelOtoceni = TDUtils.OtacejSeKCili(sekundOdMinule, Pozice, SouradniceCile, UhelOtoceni, RychlostRotace, out _);
            Pozice += TDUtils.PosunPoUhlu(UhelOtoceni, RychlostPohybu * sekundOdMinule);

            // Dosažení cíle (další dlaždice)?
            var novaPozice = Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, RychlostPohybu * sekundOdMinule);
            var vzdalenostDoCile = Vector2.Distance(novaPozice, SouradniceCile);
            //var vzdalenostDoCile = Vector2.Distance(Pozice, SouradniceCile);
            VzdalenostNaCeste = IndexPoziceNaTrase + vzdalenostDoCile / Zdroje.VelikostDlazdice; // Pozice pro účely řazení


            if (vzdalenostDoCile < 2f || vzdalenostDoCile > Vector2.Distance(Pozice, SouradniceCile))
            {
                IndexPoziceNaTrase++;
                if (IndexPoziceNaTrase < Zdroje.Aktualni.Level.Mapa.TrasaPochodu.Count - 1)
                    SouradniceCile = Zdroje.Aktualni.Level.Mapa.PoziceNaTrase(IndexPoziceNaTrase + 1);
                else
                {
                    DosahlCile = true;
                    Smazat = true;
                }
            }
            Pozice = novaPozice;

            if (Zdravi < 0)
                Smazat = true;

            ProcentoZdravi = MathHelper.Clamp(Zdravi / MaxZdravi, 0, 1);

            base.Update(sekundOdMinule);
            Ukazatel.Update(sekundOdMinule);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            Ukazatel.Draw(sb);
        }

    }

}
