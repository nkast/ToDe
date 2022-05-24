using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal abstract class Vez : HerniObjekt
    {
        public float SekundDoDalsihoVystrelu { get; set; } // Odpočet

        public float SekundMeziVystrely { get; set; }
        public float SilaStrely { get; set; }
        public float DosahStrelby { get; set; } // Poloměr rádiusu kruhu dostřelu

        public Point SouradniceNaMape { get; set; }
        public Nepritel Cil { get; set; }
        public bool Strelba { get; private set; }
        public ushort Uroven { get; private set; } = 1;
        public abstract TypVeze TypVeze { get; }


        private Kruh kruh;

        public Vez UmistiVez(Point souradniceNaMape)
        {
            SouradniceNaMape = souradniceNaMape;
            Pozice = new Vector2((souradniceNaMape.X + 0.5f) * Zdroje.VelikostDlazdice,
                                 (souradniceNaMape.Y + 0.5f) * Zdroje.VelikostDlazdice);
            UhelOtoceni = TDUtils.RND.Next(360);
            return this;
        }

        //public abstract KonfiguraceVeze KonfiguraceDalsiUrovne {get;}
        public KonfiguraceVeze KonfiguraceDalsiUrovne =>
            Zdroje.Aktualni.Level.VezDleTypu(TypVeze).ParametryVeze((ushort)(Uroven + 1));

        public virtual bool ZvysUroven()
        {
            var cfg = KonfiguraceDalsiUrovne;
            if (cfg == null) return false;
            Uroven++;
            SekundMeziVystrely = cfg.SekundMeziVystrely;
            SilaStrely = cfg.SilaStrely;
            DosahStrelby = cfg.DosahStrelby;
            RychlostRotace = cfg.RychlostRotace;
            return true;
        }

        public override void Update(float sekundOdMinule)
        {
            base.Update(sekundOdMinule);
            Strelba = false;
            if (SekundDoDalsihoVystrelu > 0)
                SekundDoDalsihoVystrelu -= sekundOdMinule;

            // Zobrazení kruhu (dostřelu) pokud je věž vybrána
            if (ObjektJeVybran)
            {
                if (kruh == null)
                    kruh = new Kruh(this);
                kruh.Update(sekundOdMinule);
            }
            else
                kruh = null;

            if (Cil == null || Cil.Smazat) return;

            UhelOtoceni = TDUtils.OtacejSeKCili(sekundOdMinule, Pozice, Cil.Pozice, UhelOtoceni,
                                                RychlostRotace, out bool muzeStrilet);
            if (muzeStrilet && SekundDoDalsihoVystrelu <= 0)
            {
                // Výstřel
                Vystrel();
                Strelba = true;
                SekundDoDalsihoVystrelu = SekundMeziVystrely;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            kruh?.Draw(sb);
        }

        protected abstract void Vystrel();

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
            SouradniceNaMape = new Point(SouradniceNaMape.Y, SouradniceNaMape.X);
        }
    }

    internal class VezKulomet : Vez
    {
        public override TypVeze TypVeze => TypVeze.Kulomet;

        public VezKulomet()
        {
            UhelKorkceObrazku = 90;
            Dlazdice = new[] {
                new DlazdiceUrceni(ZakladniDlazdice.Vez_Zakladna_1, 0.1f, false),
                new DlazdiceUrceni(ZakladniDlazdice.Vez_Kulomet_1, 0.2f, true),
            };
            var cfg = Zdroje.Aktualni.Level.VezDleTypu(TypVeze).ParametryVeze(1);
            DosahStrelby = cfg.DosahStrelby;
            RychlostRotace = cfg.RychlostRotace;
            SekundMeziVystrely = cfg.SekundMeziVystrely;
            SilaStrely = cfg.SilaStrely;
        }

        public override bool ZvysUroven()
        {
            return base.ZvysUroven();
        }

        protected override void Vystrel()
        {
            Cil.Zdravi -= SilaStrely;
        }
    }

    internal class VezRaketa : Vez
    {
        public override TypVeze TypVeze => TypVeze.Raketa;
        public float DosahExploze { get; set; }
        public float RychlostRakety { get; set; }

        public VezRaketa()
        {
            UhelKorkceObrazku = 90;
            Dlazdice = new[] {
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Zakladna_2, 0.1f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Raketa_1_Stred, 0.2f, true),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Raketa_1_Vrsek, 0.5f, true),
                };
            var cfg = Zdroje.Aktualni.Level.VezDleTypu(TypVeze).ParametryVeze<KonfiguraceVezRaketa>(1);
            DosahStrelby = cfg.DosahStrelby;
            RychlostRotace = cfg.RychlostRotace;
            SekundMeziVystrely = cfg.SekundMeziVystrely;
            SilaStrely = cfg.SilaStrely;
            DosahExploze = cfg.DosahExploze;
            RychlostRakety = cfg.RychlostRakety;
        }

        public override bool ZvysUroven()
        {
            if (!base.ZvysUroven()) return false;
            var cfg = Zdroje.Aktualni.Level.VezDleTypu(TypVeze).ParametryVeze<KonfiguraceVezRaketa>(Uroven);
            if (cfg == null) return false;
            DosahExploze = cfg.DosahExploze;
            RychlostRakety = cfg.RychlostRakety;
            return true;
        }

        public override void Update(float sekundOdMinule)
        {
            base.Update(sekundOdMinule);
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
            UhelKorkceObrazku = 90;
            this.vez = vez;
            Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Raketa_1, 0.4f) };
            RychlostPohybu = vez.RychlostRakety;
            SilaStrely = vez.SilaStrely;
            DosahExploze = vez.DosahExploze;
            //Meritko = 0.5f;
            UhelOtoceni = vez.UhelOtoceni;
            SouradniceCile = vez.Cil.Pozice;
            Pozice = vez.Pozice;
        }

        public override void Update(float sekundOdMinule)
        {
            Dopad = false;
            if (Smazat) return;

            var novaPozice = Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, RychlostPohybu * sekundOdMinule);

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
                if (Dlazdice[0].Z < 0.5f && Vector2.Distance(Pozice, vez.Pozice) > Zdroje.VelikostDlazdice * 0.6f)
                    Dlazdice[0].Z = 0.7f;
            }

            base.Update(sekundOdMinule);
        }

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
            SouradniceCile = new Vector2(SouradniceCile.Y, SouradniceCile.X);
        }

    }


}
