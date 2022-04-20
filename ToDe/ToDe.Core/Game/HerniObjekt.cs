using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal abstract class HerniObjekt
    {
        public Vector2 Pozice { get; set; }
        //public float UhelPohybu { get; set; } = 0;
        public float UhelOtoceni { get; set; } = 0;
        public float UhelKorkceObrazku { get; set; } = 0;
        public float RychlostPohybu { get; set; } = 0;
        public float RychlostRotace { get; set; } = 0;
        public float Meritko { get; set; } = 1;
        public Vector2 Stred { get; set; }
        public bool Smazat { get; set; } = false;
        public float Nepruhlednost { get; set; } = 1;

        public DlazdiceUrceni[] Dlazdice { get; set; }

        public HerniObjekt()
        {
            Stred = new Vector2(Mapa.VelikostDlazdice / 2f);
        }

        public virtual void Update(float elapsedSeconds)
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (Smazat || Nepruhlednost <= 0 || Dlazdice == null) return;
            foreach (var obr in Dlazdice)
            {
                sb.Kresli(Pozice, obr, Stred, (UhelOtoceni * (obr.Otacet ? 1 : 0)) + UhelKorkceObrazku, Meritko,
                    barva: Nepruhlednost < 1 ? Kresleni.Pruhlednost(Nepruhlednost) : (Color?)null);
            }
        }
    }

    internal class Nepritel : HerniObjekt
    {
        public float Zdravi { get; set; } = 1;
        public int IndexPoziceNaTrase { get; set; }
        public Vector2 SouradniceCile { get; set; }

        public Nepritel()
        {
            Dlazdice = new[] { new DlazdiceUrceni(17, 10, 0.1f) };
            Zdravi = 1;
            UhelKorkceObrazku = 0;
            RychlostPohybu = Mapa.VelikostDlazdice / 2; // Půl dlaždice za sekundu
            RychlostRotace = 90 * RychlostPohybu / (Mapa.VelikostDlazdice / 2); // Za dobu ujití poloviny dlaždice se otočí o 90°
            Pozice = Mapa.Aktualni.PoziceNaTrase(0);
            SouradniceCile = Mapa.Aktualni.PoziceNaTrase(1);
            IndexPoziceNaTrase = 0;
            UhelOtoceni = (int)Mapa.Aktualni.SmerDalsiTrasy(Mapa.Aktualni.TrasaPochodu[0], Mapa.Aktualni.TrasaPochodu[1]);
        }

        public override void Update(float elapsedSeconds)
        {
            // Test není-li na SouradniceCile, pokud ano, posunout IndexPoziceNaTrase a stanovit nový cíl
                // Je-li v konečném cíli, ubere život hráči a smaže se
            // Natočit a posunout k cíli (otáčí se danou rychlostí ale za chůze)

            if (Smazat) return;

            PohniSe(elapsedSeconds);

            // Dosažení cíle (další dlaždice)?
            if (Vector2.Distance(Pozice, SouradniceCile) < 2f)
            {
                IndexPoziceNaTrase++;
                if (IndexPoziceNaTrase < Mapa.Aktualni.TrasaPochodu.Count-1)
                    SouradniceCile = Mapa.Aktualni.PoziceNaTrase(IndexPoziceNaTrase+1);
                else 
                    Smazat = true;
            }

            if (Zdravi < 0)
                Smazat = true;

            base.Update(elapsedSeconds);
        }

        static float PosunPoUhlu(float uhel, float rychlost, bool x)
        {
            if (x) 
                return (float)Math.Cos(Math.PI * uhel / 180.0) * rychlost;
            return (float)Math.Sin(Math.PI * uhel / 180.0) * rychlost;
        }

        void PohniSe(float elapsedSeconds)
        {
            float ang = MathHelper.ToDegrees((float)Math.Atan2(SouradniceCile.Y - Pozice.Y, SouradniceCile.X - Pozice.X));
            float rozdilUhlu = RozdilUhlu(UhelOtoceni, ang);

            if (Math.Abs(rozdilUhlu) > RychlostRotace * elapsedSeconds)
                UhelOtoceni += Math.Sign(rozdilUhlu) * (RychlostRotace * elapsedSeconds);
            else
                UhelOtoceni = ang;

            Pozice += new Vector2(PosunPoUhlu(UhelOtoceni, RychlostPohybu * elapsedSeconds, true), 
                PosunPoUhlu(UhelOtoceni, RychlostPohybu * elapsedSeconds, false));
        }

        static float KorekceUhlu(float angle)
        {
            if (angle > 360)
                angle = angle % 360;
            while (angle < 0)
                angle += 360; // TODO: vypočíst bez cyklu
            return angle;
        }

        float RozdilUhlu(float aktualniUhel, float cilovyUhel)
        {
            aktualniUhel = KorekceUhlu(aktualniUhel);
            cilovyUhel = KorekceUhlu(cilovyUhel);

            float rozdil = cilovyUhel - aktualniUhel;

            if (rozdil > 180)
                return -(360 - rozdil);
            if (rozdil < -180)
                return rozdil + 360;
            return rozdil;
        }
    }

    internal class Vez : HerniObjekt
    {
    }

    public enum TypPolozkyNabidky
    {
        Vez1,
        Vez2,
        Text,
        Vyber, // Obdélníček značící výběr položky
    }

    internal class PolozkaNabidky : HerniObjekt
    {
        public TypPolozkyNabidky TypPolozky { get; private set; }
        public string Text { get; set; } // Pouze pro typ text
        public short PoziceVNabidce { get; private set; } // Záporná pozice = počítáno od konce

        public bool Oznacen { get => viditelny && Vyber?.PoziceVNabidce == PoziceVNabidce; }
        bool viditelny = false;
        static PolozkaNabidky Vyber;

        public PolozkaNabidky(short poziceVNabidce, TypPolozkyNabidky typPolozky)
        {
            PoziceVNabidce = poziceVNabidce;
            TypPolozky = typPolozky;

            UhelKorkceObrazku = 0;

            if (TypPolozky == TypPolozkyNabidky.Vez1)
                Dlazdice = new[] { 
                    new DlazdiceUrceni(19,  7, 0.0f, false),
                    new DlazdiceUrceni(19, 10, 0.1f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.Vez2)
                Dlazdice = new[] {
                    new DlazdiceUrceni(20, 7, 0.0f, false),
                    new DlazdiceUrceni(22, 8, 0.2f, false),
                    new DlazdiceUrceni(22, 9, 0.5f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.Vyber)
            {
                Dlazdice = new[] { new DlazdiceUrceni(15, 0, 0.9f, false) };
            }
            if (TypPolozky != TypPolozkyNabidky.Text)
            {
                Pozice = new Vector2((PoziceVNabidce + 0.5f) * Mapa.VelikostDlazdice,
                                     (Mapa.Aktualni.Radku + 0.5f) * Mapa.VelikostDlazdice);
            }

            if (Vyber == null && TypPolozky != TypPolozkyNabidky.Vyber)
            {
                Vyber = new PolozkaNabidky(0, TypPolozkyNabidky.Vyber);
            }
        }

        public void Update(float elapsedSeconds, Vector2 klik)
        {
            Update(elapsedSeconds);

            if (TypPolozky != TypPolozkyNabidky.Text && klik != Vector2.Zero)
            {
                if (new Rectangle(PoziceVNabidce * Mapa.VelikostDlazdice, 
                                  Mapa.Aktualni.Radku * Mapa.VelikostDlazdice,
                                  Mapa.VelikostDlazdice, Mapa.VelikostDlazdice).Contains(klik))
                {
                    if (Vyber.PoziceVNabidce == PoziceVNabidce)
                        Vyber.viditelny = !Vyber.viditelny;
                    else
                    {
                        Vyber.viditelny = true;
                        Vyber.PoziceVNabidce = PoziceVNabidce;
                        Vyber.Pozice = new Vector2((PoziceVNabidce + 0.5f) * Mapa.VelikostDlazdice,
                                     (Mapa.Aktualni.Radku + 0.5f) * Mapa.VelikostDlazdice);
                    }
                }
            }
        }

        public override void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);

            if (TypPolozky == TypPolozkyNabidky.Text)
            {
                var rozmery = Mapa.Pismo.MeasureString(Text) * Meritko;
                Stred = new Vector2(PoziceVNabidce < 0 ? rozmery.X : 0, rozmery.Y * 0.5f);
                Pozice = new Vector2((PoziceVNabidce < 0 
                                        ? Mapa.Aktualni.Sloupcu - 0.1f 
                                        : PoziceVNabidce + 0.1f)
                                     * Mapa.VelikostDlazdice,
                                    (Mapa.Aktualni.Radku + 0.5f) * Mapa.VelikostDlazdice);
            }
        }

        public void DrawVyber(SpriteBatch sb)
        {
            if (Vyber.viditelny)
                Vyber.Draw(sb);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (TypPolozky == TypPolozkyNabidky.Text)
            {
                sb.DrawString(Mapa.Pismo, Text, Pozice, Color.White, 
                    UhelOtoceni, Stred, Meritko, SpriteEffects.None, 0);
            } else
                base.Draw(sb);
        }
    }

    internal class Raketa : HerniObjekt
    {

    }

    internal class Dira : HerniObjekt
    {
        public float RychlostMizeni { get; set; } // -% za s

        public Dira()
        {
            Dlazdice = new[] { new DlazdiceUrceni(21, 0, 0.000001f) };
        }

        public override void Update(float elapsedSeconds)
        {
            Nepruhlednost -= RychlostMizeni * elapsedSeconds;
            if (Nepruhlednost <= 0)
                Smazat = true;

            base.Update(elapsedSeconds);
        }
    }
}
