using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal enum TypNabidky
    {
        Zakladni, // Nic neni oznaceno
        ProDlazdici, // Je označena prázdná dlaždice na ploše
        ProVez, // Je označena njěaká věž
        ProPrekazku, // Je označena překážka na mapě
    }

    internal class OvladaciPanel
    {
        List<PolozkaNabidky> nabidka;
        Dictionary<TypNabidky, List<PolozkaNabidky>> nabidky;

        public TypNabidky AktualniNabidka { get; set; }

        public PolozkaNabidky KliknutoNa { get; private set; }

        public PolozkaNabidky TextZivotu { get; private set; }
        public PolozkaNabidky TextFinance { get; private set; }

        public PolozkaNabidky TextCenaVezeKluomet { get; private set; }
        public PolozkaNabidky TextCenaVezeRaketa { get; private set; }
        
        public PolozkaNabidky TextCenaDemolice { get; private set; }
        
        public PolozkaNabidky Pauza { get; private set; }

        public OvladaciPanel()
        {          
            TextFinance = new PolozkaNabidky(-3, TypPolozkyNabidky.Text);
            Pauza = new PolozkaNabidky(-2, TypPolozkyNabidky.Pauza);
            TextZivotu = new PolozkaNabidky(-1, TypPolozkyNabidky.Text);

            TextCenaVezeKluomet = new PolozkaNabidky(1, TypPolozkyNabidky.Text);
            TextCenaVezeRaketa = new PolozkaNabidky(3, TypPolozkyNabidky.Text);
            TextCenaDemolice = new PolozkaNabidky(1, TypPolozkyNabidky.Text);

            nabidky = new Dictionary<TypNabidky, List<PolozkaNabidky>>() 
            {
                 { TypNabidky.Zakladni,
                    new List<PolozkaNabidky>()
                    {
                        TextFinance,
                        Pauza,
                        TextZivotu,
                    }
                },
                { TypNabidky.ProDlazdici, 
                    new List<PolozkaNabidky>()
                    {
                        new PolozkaNabidky(0, TypPolozkyNabidky.VezKulomet),
                        TextCenaVezeKluomet,
                        new PolozkaNabidky(2, TypPolozkyNabidky.VezRaketa),
                        TextCenaVezeRaketa,
                        TextFinance,
                        Pauza,
                        TextZivotu,
                    } 
                },
                { TypNabidky.ProVez,
                    new List<PolozkaNabidky>()
                    {
                        new PolozkaNabidky(0, TypPolozkyNabidky.Vymazat),
                        TextCenaDemolice, // Cena +- za delete
                        //new PolozkaNabidky(2, TypPolozkyNabidky.VezRaketa), // Upgrade
                        //new PolozkaNabidky(3, TypPolozkyNabidky.Text), // Parametry update (včetně ceny)
                        // Víc textů...
                        TextFinance,
                        Pauza,
                        TextZivotu,
                    }
                },
            };

            PrepniNabidku(TypNabidky.Zakladni);
        }

        public void PrepniNabidku(TypNabidky typ)
        {
            if (typ != AktualniNabidka || nabidka == null)
            {
                nabidka = nabidky[typ];
                AktualniNabidka = typ;
            }
        }


        public void Update(float sekundOdMinule, Vector2 klik)
        {
            KliknutoNa = null;
            foreach (var polozka in nabidka)
            {
                polozka.Update(sekundOdMinule, klik);
                if (polozka.Kliknuto)
                    KliknutoNa = polozka;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            nabidka.ForEach(x => x.Draw(sb));
        }

        public void TranspozicePozice()
        {
            nabidka.ForEach(x => x.TranspozicePozice());
        }
    }


    public enum TypPolozkyNabidky
    {
        VezKulomet,
        VezRaketa,
        Pauza,
        Text,
        Vymazat,
    }

    internal class PolozkaNabidky : HerniObjekt
    {
        const float MaxMeritkoTextu = 0.5f;

        public TypPolozkyNabidky TypPolozky { get; private set; }
        public string Text { get; set; } // Pouze pro typ text
        public ushort SirkaTextu { get; set; } = 0; // Kolik dlaždic může text zabírat maximálně (0 = neomezovat)
        public float SirkaOkraje { get; set; } = 0; // Mezera obsahu dlaždice od jejího okraje (padding)
        public short PoziceVNabidce { get; private set; } // Záporná pozice = počítáno od konce
        public bool Kliknuto { get; set; }

        public PolozkaNabidky(short poziceVNabidce, TypPolozkyNabidky typPolozky)
        {
            PoziceVNabidce = poziceVNabidce;
            TypPolozky = typPolozky;

            UhelKorkceObrazku = 0;
            SirkaOkraje = 16;

            // Grafika netextových typů
            if (TypPolozky == TypPolozkyNabidky.VezKulomet)
                Dlazdice = new[] {
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Zakladna_1, 0.0f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Kulomet_1, 0.1f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.VezRaketa)
                Dlazdice = new[] {
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Zakladna_2, 0.0f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Raketa_1_Stred, 0.2f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Raketa_1_Vrsek, 0.5f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.Vymazat)
            {
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Kos, 0.0f, false), };
                Barva = Color.Silver;
                SirkaOkraje = 48;
            }
            else if (TypPolozky == TypPolozkyNabidky.Pauza)
            {
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Pauza, 0.0f, false), };
                SirkaOkraje = 48;

            }
            else if (TypPolozky == TypPolozkyNabidky.Text)
            {
                Meritko = MaxMeritkoTextu; // Nastavení měřítka textu a výběrovému políčku
                Text = " ";
            }
        }

        public void Update(float sekundOdMinule, Vector2 klik)
        {
            Kliknuto = false;

            Update(sekundOdMinule);

            if (TypPolozky == TypPolozkyNabidky.Text) return; // Text se řeší v předchozím příkazu

            // Pozice - levý horní roh dlaždice
            var pozice = new Point((PoziceVNabidce >= 0 ? PoziceVNabidce :
                                Zdroje.Aktualni.Level.Mapa.Sloupcu + PoziceVNabidce) * Zdroje.VelikostDlazdice,
                                Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice);

            // Pozice netextových položek nabídky - střed dlaždice
            Pozice = new Vector2(pozice.X + 0.5f * Zdroje.VelikostDlazdice, pozice.Y + 0.5f * Zdroje.VelikostDlazdice);

            if (klik != Vector2.Zero)
                if (new Rectangle(pozice.X, pozice.Y, Zdroje.VelikostDlazdice, Zdroje.VelikostDlazdice).Contains(klik))
                    Kliknuto = true;

            // Výpočet měřítka vzhledem k okrajům
            Meritko = (Zdroje.VelikostDlazdice - SirkaOkraje) / (float)Zdroje.VelikostDlazdice;
        }

        public override void Update(float sekundOdMinule)
        {
            base.Update(sekundOdMinule);

            if (TypPolozky != TypPolozkyNabidky.Text) return;

            var rozmery = Zdroje.Obsah.Pismo.MeasureString(Text);
            Stred = new Vector2(PoziceVNabidce < 0 ? rozmery.X : 0, rozmery.Y * 0.5f);
            Pozice = new Vector2((PoziceVNabidce < 0
                                    ? Zdroje.Aktualni.Level.Mapa.Sloupcu + PoziceVNabidce + 1 //- 0.1f
                                    : PoziceVNabidce) //+ 0.1f)
                                    * Zdroje.VelikostDlazdice + SirkaOkraje * Math.Sign(PoziceVNabidce),
                                (Zdroje.Aktualni.Level.Mapa.Radku + 0.5f) * Zdroje.VelikostDlazdice);

            // Zmenšení měřítka (je-li to za potřebí), aby se text vešel do zadaného počtu dlaždic na šířku
            if (rozmery.X * MaxMeritkoTextu > Zdroje.VelikostDlazdice - SirkaOkraje)
                Meritko = (Zdroje.VelikostDlazdice - SirkaOkraje) / rozmery.X;
            else
                Meritko = MaxMeritkoTextu;
        }


        public override void Draw(SpriteBatch sb)
        {
            if (TypPolozky == TypPolozkyNabidky.Text)
            {
                sb.DrawString(Zdroje.Obsah.Pismo, Text??"", Pozice, Barva,
                    0, Stred, Meritko, SpriteEffects.None, 0);
            }
            else
                base.Draw(sb);
        }

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
        }
    }

}
