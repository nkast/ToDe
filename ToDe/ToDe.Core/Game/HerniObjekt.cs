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

    internal abstract class AnimovanyHerniObjekt : HerniObjekt
    {
        protected Texture2D textura;

        public float Z { get; set; } = 0;

        public int PocetObrazkuSirka { get; private set; } = 1;
        public int PocetObrazkuVyska { get; private set; } = 1;
        public int IndexObrazku { get; set; } = 0;
        public float RychlostAnimace { get; set; } = 0;
        public bool OpakovatAnimaci { get; set; } = false;

        public int SirkaObrzaku { get; private set; }
        public int VyskaObrzaku { get; private set; }
        public Rectangle VyrezZTextury { get; set; }

        private double postupAnimace = 0;

        public AnimovanyHerniObjekt(Texture2D textura, int pocetObrazkuSirka = 1, int pocetObrazkuVyska = 1)
        {
            this.textura = textura;
            PocetObrazkuSirka = pocetObrazkuSirka;
            PocetObrazkuVyska = pocetObrazkuVyska;
            SirkaObrzaku = textura.Width / PocetObrazkuSirka;
            VyskaObrzaku = textura.Height / PocetObrazkuVyska;
            Stred = new Vector2(SirkaObrzaku / 2.0f, VyskaObrzaku * 0.5f);
        }

        public override void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);

            // Animace
            if (RychlostAnimace > 0)
            {
                if (postupAnimace >= PocetObrazkuSirka * PocetObrazkuVyska)
                {
                    if (OpakovatAnimaci)
                    {
                        IndexObrazku = 0;
                        postupAnimace = 0;
                    }
                    else
                    {
                        Smazat = true;
                        return;
                    }
                }
                else
                    IndexObrazku = (int)postupAnimace;
                postupAnimace += RychlostAnimace * elapsedSeconds;
            }

            // Výřez z obrázku
            VyrezZTextury = new Rectangle(
                SirkaObrzaku * (IndexObrazku % PocetObrazkuSirka),
                VyskaObrzaku * (IndexObrazku / PocetObrazkuSirka),
                SirkaObrzaku, VyskaObrzaku);
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(sb);        

            sb.Kresli(Pozice, VyrezZTextury, Stred, UhelOtoceni + UhelKorkceObrazku, Meritko, Z, SpriteEffects.None,
                   Nepruhlednost < 1 ? Kresleni.Pruhlednost(Nepruhlednost) : (Color?)null, textura);
        }
    }

    internal class Exploze : AnimovanyHerniObjekt
    {
        public Exploze(Raketa raketa) : base(Mapa.Textury.Exploze, 8, 6)
        {
            Pozice = raketa.Pozice;
            UhelOtoceni = TDUtils.RND.Next(360);
            Meritko = 0.75f;
            Z = 0.5f;
            RychlostAnimace = 25f;
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
                var rozmery = Mapa.Textury.Pismo.MeasureString(Text) * Meritko;
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
                sb.DrawString(Mapa.Textury.Pismo, Text, Pozice, Color.White, 
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
            if (SekundDoDalsihoVystrelu > 0)
                SekundDoDalsihoVystrelu -= elapsedSeconds;

            if (Cil == null || Cil.Smazat) return;

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
            RychlostRotace = 90;
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
        public float DosahExploze { get; set; }

        public VezRaketa()
        {
            UhelKorkceObrazku = 90;
            Dlazdice = new[] {
                    new DlazdiceUrceni(20, 7, 0.1f, false),
                    new DlazdiceUrceni(22, 8, 0.2f, true),
                    new DlazdiceUrceni(22, 9, 0.5f, true),
                };
            DosahStrelby = Mapa.VelikostDlazdice * 4.1f;
            RychlostRotace = 45;
            SekundMeziVystrely = 2f;
            SilaStrely = 0.3f;
            DosahExploze = Mapa.VelikostDlazdice * 1.5f;
        }

        public override void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);
            if (!Strelba && SekundDoDalsihoVystrelu <= 0)
                Dlazdice[1].Vykreslovat = true;
        }

        protected override void Vystrel()
        {
            Dlazdice[1].Vykreslovat = false;
        }
    }


    internal class Raketa : HerniObjekt
    {
        VezRaketa vez;
        public Vector2 SouradniceCile { get; set; }
        public bool Dopad { get; private set; } = false;
        public float SilaStrely { get; set; } // Kolik ubere života v epicentru
        public float DosahExploze { get; set; } // Jak až daleko bude zraňovat nepřátele

        public Raketa(VezRaketa vez)
        {
            this.vez = vez;
            Dlazdice = new[] { new DlazdiceUrceni(22, 10, 0.4f) };
            RychlostPohybu = Mapa.VelikostDlazdice * 2.0f;
            UhelKorkceObrazku = 90;
            SilaStrely = vez.SilaStrely;
            DosahExploze = vez.DosahExploze;
            //Meritko = 0.5f;
            UhelOtoceni = vez.UhelOtoceni;
            SouradniceCile = vez.Cil.Pozice;
            Pozice = vez.Pozice;
        }

        public override void Update(float elapsedSeconds)
        {
            Dopad = false;
            if (Smazat) return;

            var novaPozice = Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, RychlostPohybu * elapsedSeconds);

            // Dosažení cíle (další dlaždice)?
            var vzdalenostDoCile = Vector2.Distance(novaPozice, SouradniceCile);

            if (vzdalenostDoCile < 2f || vzdalenostDoCile > Vector2.Distance(Pozice, SouradniceCile))
            {
                Dopad = true;
                Smazat = true;
            }
            else
            {
                Pozice = novaPozice;
                // Když raketa vylétne z věže, posunout ji výše, aby létala nad ostatními věžemi
                if (Dlazdice[0].Z < 0.5f && Vector2.Distance(Pozice, vez.Pozice) > Mapa.VelikostDlazdice * 0.6f)
                    Dlazdice[0].Z = 0.7f;
            }

            base.Update(elapsedSeconds);
        }

    }

    internal abstract class MizejiciObraz : HerniObjekt
    {
        public float RychlostMizeni { get; set; } // -% za s

        public override void Update(float elapsedSeconds)
        {
            Nepruhlednost -= RychlostMizeni * elapsedSeconds;
            if (Nepruhlednost <= 0)
            {
                Nepruhlednost = 0;
                Smazat = true;
            }

            base.Update(elapsedSeconds);
        }
    }

    internal class Dira : MizejiciObraz
    {
        public Dira()
        {
            Dlazdice = new[] { new DlazdiceUrceni(21, 0, 0.000001f) };
            RychlostMizeni = (float)(TDUtils.RND.NextDouble() * 0.4 + 0.2);
            UhelOtoceni = TDUtils.RND.Next(360);
        }
    }

    internal class PlamenStrelby : MizejiciObraz
    {
        VezKulomet vez;

        public PlamenStrelby(VezKulomet vez)
        {
            this.vez = vez;
            Dlazdice = new[] { new DlazdiceUrceni(21, 12, 0.61f) };
            RychlostMizeni = Math.Max(1 / vez.SekundMeziVystrely, 2.0f);
            UhelKorkceObrazku = 90;
            Meritko = 0.5f;
            UhelOtoceni = vez.UhelOtoceni;
            Pozice = vez.Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, Mapa.VelikostDlazdice * 0.55f);
        }

        public override void Update(float elapsedSeconds)
        {
            UhelOtoceni = vez.UhelOtoceni;
            Pozice = vez.Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, Mapa.VelikostDlazdice * 0.55f);
            base.Update(elapsedSeconds);
        }
    }
}
