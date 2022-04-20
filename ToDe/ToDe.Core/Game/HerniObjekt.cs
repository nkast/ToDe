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
        public float VzdalenostNaCeste { get; private set; } 

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

            UhelOtoceni = TDUtils.OtacejSeKCili(elapsedSeconds, Pozice, SouradniceCile, UhelOtoceni, RychlostRotace, out _);
            Pozice += TDUtils.PosunPoUhlu(UhelOtoceni, RychlostPohybu * elapsedSeconds);

            // Dosažení cíle (další dlaždice)?
            var vzdalenostDoCile = Vector2.Distance(Pozice, SouradniceCile);
            VzdalenostNaCeste = IndexPoziceNaTrase + vzdalenostDoCile / Mapa.VelikostDlazdice; // Pozice pro účely řazení

            if (vzdalenostDoCile < 2f)
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
 
    }


    public enum TypPolozkyNabidky
    {
        VezKulomet,
        VezRaketa,
        Text,
        Vyber, // Obdélníček značící výběr položky
    }

    internal class PolozkaNabidky : HerniObjekt
    {
        public TypPolozkyNabidky TypPolozky { get; private set; }
        public string Text { get; set; } // Pouze pro typ text
        public short PoziceVNabidce { get; private set; } // Záporná pozice = počítáno od konce

        public bool Oznacen { get => Vyber?.viditelny == true && Vyber?.PoziceVNabidce == PoziceVNabidce; }
        bool viditelny = false;
        static PolozkaNabidky Vyber;

        public PolozkaNabidky(short poziceVNabidce, TypPolozkyNabidky typPolozky)
        {
            PoziceVNabidce = poziceVNabidce;
            TypPolozky = typPolozky;

            UhelKorkceObrazku = 0;

            if (TypPolozky == TypPolozkyNabidky.VezKulomet)
                Dlazdice = new[] { 
                    new DlazdiceUrceni(19,  7, 0.0f, false),
                    new DlazdiceUrceni(19, 10, 0.1f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.VezRaketa)
                Dlazdice = new[] {
                    new DlazdiceUrceni(20, 7, 0.0f, false),
                    new DlazdiceUrceni(22, 8, 0.2f, false),
                    new DlazdiceUrceni(22, 9, 0.5f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.Vyber)
            {
                Dlazdice = new[] { new DlazdiceUrceni(15, 0, 0.9f, false) };
            }

            if (TypPolozky != TypPolozkyNabidky.Text && TypPolozky != TypPolozkyNabidky.Vyber)
            {
                Pozice = new Vector2((PoziceVNabidce + 0.5f) * Mapa.VelikostDlazdice,
                                     (Mapa.Aktualni.Radku + 0.5f) * Mapa.VelikostDlazdice);
                Meritko = 0.75f;
            }

            if (Vyber == null && TypPolozky != TypPolozkyNabidky.Vyber)
            {
                Vyber = new PolozkaNabidky(-1, TypPolozkyNabidky.Vyber);
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


    internal abstract class Vez : HerniObjekt
    {
        public float SekundDoDalsihoVystrelu { get; set; } // Odpočet
        public float SekundMeziVystrely { get; set; }
        public float SilaStrely { get; set; } 
        public float DosahStrelby { get; set; } // Poloměr rádiusu kruhu dostřelu
        public Point SouradniceNaMape { get; set; }
        public Nepritel Cil { get; set; }
        public bool Strelba { get; private set; }

        public Vez UmistiVez(Point souradniceNaMape)
        {
            SouradniceNaMape = souradniceNaMape;
            Pozice = new Vector2((souradniceNaMape.X + 0.5f) * Mapa.VelikostDlazdice,
                                 (souradniceNaMape.Y + 0.5f) * Mapa.VelikostDlazdice);
            return this;
        }

        public override void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);
            if (Cil == null) return;
            SekundDoDalsihoVystrelu -= elapsedSeconds;
            UhelOtoceni = TDUtils.OtacejSeKCili(elapsedSeconds, Pozice, Cil.Pozice, UhelOtoceni, 
                                                RychlostRotace, out bool muzeStrilet);
            if (muzeStrilet && SekundDoDalsihoVystrelu <= 0)
            {
                // Výstřel
                Vystrel();
                Strelba = true;
                SekundDoDalsihoVystrelu = SekundMeziVystrely;
            } else 
                Strelba = false;
        }

        protected abstract void Vystrel();
    }

    internal class VezKulomet : Vez
    {
        public VezKulomet()
        {
            UhelKorkceObrazku = 90;
            Dlazdice = new[] {
                new DlazdiceUrceni(19,  7, 0.1f, false),
                new DlazdiceUrceni(19, 10, 0.2f, true),
            };
            DosahStrelby = Mapa.VelikostDlazdice * 2.1f;
            RychlostRotace = 135;
            SekundMeziVystrely = 0.5f;
            SilaStrely = 0.01f;
        }

        protected override void Vystrel()
        {
            Cil.Zdravi -= SilaStrely;
        }
    }

    internal class VezRaketa : Vez
    {
        public VezRaketa()
        {
            UhelKorkceObrazku = 90;
            Dlazdice = new[] {
                    new DlazdiceUrceni(20, 7, 0.1f, false),
                    new DlazdiceUrceni(22, 8, 0.2f, true),
                    new DlazdiceUrceni(22, 9, 0.5f, true),
                };
            DosahStrelby = Mapa.VelikostDlazdice * 4.1f;
            RychlostRotace = 90;
            SekundMeziVystrely = 1f;
            SilaStrely = 0.75f;
        }

        protected override void Vystrel()
        {
        }
    }


    internal class Raketa : HerniObjekt
    {

    }

    internal abstract class MizejiciObraz : HerniObjekt
    {
        public float RychlostMizeni { get; set; } // -% za s

        public override void Update(float elapsedSeconds)
        {
            Nepruhlednost -= RychlostMizeni * elapsedSeconds;
            if (Nepruhlednost <= 0)
                Smazat = true;

            base.Update(elapsedSeconds);
        }
    }

    internal class Dira : MizejiciObraz
    {
        public Dira()
        {
            Dlazdice = new[] { new DlazdiceUrceni(21, 0, 0.000001f) };
            RychlostMizeni = 0.075f;
        }
    }

    internal class PlamenStrelby : MizejiciObraz
    {
        Vez vez;

        public PlamenStrelby(Vez vez)
        {
            this.vez = vez;
            Dlazdice = new[] { new DlazdiceUrceni(21, 12, 0.61f) };
            RychlostMizeni = 2.0f;
            UhelKorkceObrazku = 90;
            Meritko = 0.5f;
            UhelOtoceni = vez.UhelOtoceni;
            Pozice = vez.Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, Mapa.VelikostDlazdice * 0.55f);
        }

        public override void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);
            UhelOtoceni = vez.UhelOtoceni;
            Pozice = vez.Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, Mapa.VelikostDlazdice * 0.55f);
        }
    }
}
