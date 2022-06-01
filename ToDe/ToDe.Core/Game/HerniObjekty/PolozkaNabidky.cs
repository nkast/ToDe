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

    //internal class PolozkaMenu
    //{
    //    public PolozkaNabidky Polozka { get; private set; }
    //}

    internal class OvladaciPanel
    {
        public const int SirkaNabidky = 10; // vyjádřeno v počtu dlaždic
        public static readonly Color Pozadi = Color.FromNonPremultiplied(30, 30, 30, 255);

        Rectangle polohaMenu;
        List<PolozkaNabidky> nabidka;
        Dictionary<TypNabidky, List<PolozkaNabidky>> nabidky;

        public TypNabidky AktualniNabidka { get; set; }

        public PolozkaNabidky KliknutoNa { get; private set; }
        public bool KliknutoDoNabidky { get; private set; }
        public float Meritko { get; set; }

        //public PolozkaNabidky TextZivotu { get; private set; }
        //public PolozkaNabidky TextFinance { get; private set; }
        public PolozkaNabidky TextFinanceAZivotu { get; private set; }

        public PolozkaNabidky TextCenaVezeKluomet { get; private set; }
        public PolozkaNabidky TextCenaVezeRaketa { get; private set; }

        PolozkaNabidkyObrazekSTextem demolice;
        public PolozkaNabidky TextCenaDemolice { get; private set; }

        PolozkaNabidkyObrazekSTextem upgrade;
        public PolozkaNabidky TextCenaUpgrade { get; private set; }

        public PolozkaNabidky Pauza { get; private set; }

        public PolozkaNabidky ProDlazdiciVezKulomet { get; private set; }
        public PolozkaNabidky ProDlazdiciVezRaketa { get; private set; }

        public PolozkaNabidky UpgradeText1 { get; private set; }
        public PolozkaNabidky UpgradeText2 { get; private set; }
        public PolozkaNabidky UpgradeText3 { get; private set; }

        public OvladaciPanel()
        {          
            Pauza = new PolozkaNabidky(-2, TypPolozkyNabidky.Pauza);
            //TextFinance = new PolozkaNabidky(-3, TypPolozkyNabidky.Text);
            //TextZivotu = new PolozkaNabidky(-1, TypPolozkyNabidky.Text);
            
            TextFinanceAZivotu = new PolozkaNabidky(-1, TypPolozkyNabidky.Text);

            TextCenaVezeKluomet = new PolozkaNabidky(1, TypPolozkyNabidky.Text);
            TextCenaVezeRaketa = new PolozkaNabidky(3, TypPolozkyNabidky.Text);
            //TextCenaDemolice = new PolozkaNabidky(0, TypPolozkyNabidky.Vymazat) { VyskaTextuPodObrazkem = 0.33f };
            demolice = new PolozkaNabidkyObrazekSTextem(0, TypPolozkyNabidky.Vymazat);
            TextCenaDemolice = demolice.TextovaCast;
            upgrade = new PolozkaNabidkyObrazekSTextem(1, TypPolozkyNabidky.Upgrade);
            TextCenaUpgrade = upgrade.TextovaCast;

            nabidky = new Dictionary<TypNabidky, List<PolozkaNabidky>>() 
            {
                 { TypNabidky.Zakladni,
                    new List<PolozkaNabidky>()
                    {
                        //TextFinance,
                        Pauza,
                        //TextZivotu,
                        TextFinanceAZivotu,
                    }
                },
                { TypNabidky.ProDlazdici, 
                    new List<PolozkaNabidky>()
                    {
                        (ProDlazdiciVezKulomet = new PolozkaNabidky(0, TypPolozkyNabidky.VezKulomet)),
                        TextCenaVezeKluomet,
                        (ProDlazdiciVezRaketa = new PolozkaNabidky(2, TypPolozkyNabidky.VezRaketa)),
                        TextCenaVezeRaketa,
                        //TextFinance,
                        Pauza,
                        //TextZivotu,
                        TextFinanceAZivotu,
                    }
                },
                { TypNabidky.ProVez,
                    new List<PolozkaNabidky>()
                    {
                        //new PolozkaNabidky(0, TypPolozkyNabidky.Vymazat),
                        demolice, // Cena +- za delete
                        upgrade,
                        //new PolozkaNabidky(2, TypPolozkyNabidky.VezRaketa), // Upgrade
                        (UpgradeText1 = new PolozkaNabidky(2, TypPolozkyNabidky.Text) { SirkaTextu = 2 } ),
                        (UpgradeText2 = new PolozkaNabidky(4, TypPolozkyNabidky.Text) { SirkaTextu = 2 } ),
                        (UpgradeText3 = new PolozkaNabidky(6, TypPolozkyNabidky.Text) { SirkaTextu = 2 } ),
                        // Víc textů...
                        //TextFinance,
                        Pauza,
                        //TextZivotu,
                        TextFinanceAZivotu,
                    }
                },
                { TypNabidky.ProPrekazku,
                    new List<PolozkaNabidky>()
                    {
                        //new PolozkaNabidky(0, TypPolozkyNabidky.Vymazat),
                        demolice,
                        //TextFinance,
                        Pauza,
                        //TextZivotu,
                        TextFinanceAZivotu,
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

        public void PrepniNabidku(TypNabidky typ, Vez vez)
        {
            PrepniNabidku(typ);
            ushort novaUroven = (ushort)(vez.Uroven + 1);
            KonfiguraceVeze novaCfg = null, staraCfg = null;

            staraCfg = Zdroje.Aktualni.Level.VezDleTypu(vez.TypVeze).ParametryVeze(vez.Uroven);
            novaCfg = Zdroje.Aktualni.Level.VezDleTypu(vez.TypVeze).ParametryVeze(novaUroven);
            
            switch (vez.TypVeze)
            {
                case TypVeze.Kulomet:
                    //staraCfg = KonfiguraceVezKulomet.ParametryVeze(vez.Uroven);
                    //novaCfg = KonfiguraceVezKulomet.ParametryVeze(novaUroven);
                    UpgradeText3.Text = "";
                    break;
                case TypVeze.Raketa:
                    //staraCfg = KonfiguraceVezRaketa.ParametryVeze(vez.Uroven);
                    //novaCfg = KonfiguraceVezRaketa.ParametryVeze(novaUroven);
                    if (novaCfg != null)
                        UpgradeText3.Text = String.Format("Rakety\nRychlost: {0}\nDosah: {1}",
                            UpgradeHodnota(((KonfiguraceVezRaketa)staraCfg).RychlostRakety, ((KonfiguraceVezRaketa)novaCfg).RychlostRakety),
                            UpgradeHodnota(((KonfiguraceVezRaketa)staraCfg).DosahExploze, ((KonfiguraceVezRaketa)novaCfg).DosahExploze));
                    break;
                default: break;
            }
            if (novaCfg != null)
            {
                upgrade.Viditelne = true;
                //upgrade.TextovaCast.Text = "$" + Math.Abs(novaCfg.Cena).ToString();
                upgrade.TextovaCast.NastavTextCeny(-novaCfg.Cena);
                UpgradeText1.Text = String.Format("Upgrade {0}->{1}\nDostrel: {2}\nOtaceni: {3}",
                    vez.Uroven, novaUroven,
                    UpgradeHodnota(staraCfg.DosahStrelby, novaCfg.DosahStrelby),
                    UpgradeHodnota(staraCfg.RychlostRotace, novaCfg.RychlostRotace));
                UpgradeText2.Text = String.Format("Nabijeni {0}\nStrilen: {1}\nSila: {2}",
                    UpgradeHodnota(staraCfg.SekundMeziVystrely, novaCfg.SekundMeziVystrely),
                    novaCfg.PocetStrilen,
                    UpgradeHodnota(staraCfg.SilaStrely, novaCfg.SilaStrely));
            }
            else
            {
                VymazUpgradeTexty();
                UpgradeText1.Text = $"Vez max urovne {vez.Uroven}";
            }
        }

        static string UpgradeHodnota(float stara, float nova)
        {
            float posun = (float)Math.Round(((nova - stara) / stara)*100, 0);
            return $"{(posun < 0 ? "-" : "+")}{Math.Abs(posun)}%";
        }

        void VymazUpgradeTexty()
        {
            UpgradeText1.Text = "";
            UpgradeText2.Text = "";
            UpgradeText3.Text = "";
            upgrade.Viditelne = false;
        }


        public void Update(float sekundOdMinule, Vector2 klik, Rectangle vyrez)
        {
            this.polohaMenu = new Rectangle(
                vyrez.X, 
                (int)(vyrez.Y + vyrez.Height / Meritko) - Zdroje.VelikostDlazdice,
                (int)Math.Ceiling(vyrez.Width / Meritko), 
                Zdroje.VelikostDlazdice);
            KliknutoNa = null;
            KliknutoDoNabidky = klik.Y >= polohaMenu.Y;

            foreach (var polozka in nabidka)
            {
                polozka.Update(sekundOdMinule, klik, polohaMenu);
                if (polozka.Kliknuto && (polozka as PolozkaNabidkyObrazekSTextem)?.Viditelne != false)
                    KliknutoNa = polozka;
            }
        }
        
        public void Draw(SpriteBatch sb)
        { 
            sb.Draw(Zdroje.Obsah.Ukazatel.Grafika, polohaMenu.Plus(plusSirka: 1, plusVyska: 1),
                new Rectangle(Zdroje.Obsah.Ukazatel.Okraj, Zdroje.Obsah.Ukazatel.Okraj, 
                   Zdroje.Obsah.Ukazatel.VelikostDlazdice.X, Zdroje.Obsah.Ukazatel.VelikostDlazdice.Y),
                Pozadi, 0, Vector2.Zero, SpriteEffects.None, 0.899f); // Pozadí
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
        Upgrade,
    }

    internal class PolozkaNabidkyObrazekSTextem : PolozkaNabidky
    {
        //public PolozkaNabidky Obrazek { get; set; }
        public PolozkaNabidky TextovaCast { get; set; }
        public float VyskaTextuPodObrazkem { get; set; } // % dlaždice
        public float MezeraMezi { get; set; }
        public bool Viditelne { get; set; } = true;

        public override string Text { get => TextovaCast.Text; set => TextovaCast.Text = value; }
        public override ushort SirkaTextu { get => TextovaCast.SirkaTextu; set => TextovaCast.SirkaTextu = value; }
        public Color BarvaTextu { get => TextovaCast.Barva; set => TextovaCast.Barva = value; }

        MezeryOdOkraju VychoziOkraje;

        public PolozkaNabidkyObrazekSTextem(short poziceVNabidce, TypPolozkyNabidky typPolozky) 
            : base(poziceVNabidce, typPolozky)
        {
            TextovaCast = new PolozkaNabidky(poziceVNabidce, TypPolozkyNabidky.Text);
            VychoziOkraje = new MezeryOdOkraju(8, 8, 8, 4);
            MezeraMezi = 8;
            VyskaTextuPodObrazkem = 0.35f;
            TextovaCast.ZarovnatTextHorizontalneNaStred = true;
        }

        public override void Update(float sekundOdMinule, Vector2 klik, Rectangle vyrez)
        {
            if (Viditelne)
            {
                TextovaCast.Vyrez = vyrez;
                base.Update(sekundOdMinule, klik, vyrez);
            }
        }

        public override void Update(float sekundOdMinule)
        {
            if (!Viditelne) return;
            Okraje = new MezeryOdOkraju(VychoziOkraje, 
                dole: Zdroje.VelikostDlazdice * VyskaTextuPodObrazkem + MezeraMezi * 0.5f);
            base.Update(sekundOdMinule);

            TextovaCast.Okraje = new MezeryOdOkraju(VychoziOkraje, 
                nahore: Zdroje.VelikostDlazdice * (1 - VyskaTextuPodObrazkem) + MezeraMezi * 0.5f);
            TextovaCast.Update(sekundOdMinule);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!Viditelne) return;
            base.Draw(sb);
            TextovaCast.Draw(sb);
        }
    }

    internal class PolozkaNabidky : HerniObjekt
    {
        const float MaxMeritkoTextu = 0.5f;

        public TypPolozkyNabidky TypPolozky { get; private set; }
        
        public MezeryOdOkraju Okraje { get; set; } = 0; // Mezera obsahu dlaždice od jejího okraje (padding)
        public bool Skryta { get; set; } = false;

        public virtual string Text { get; set; } // Pouze pro typ text
        public virtual ushort SirkaTextu { get; set; } = 1; // Kolik dlaždic může text zabírat maximálně 
        public bool ZarovnatTextHorizontalneNaStred { get; set; } = false;
                
        public short PoziceVNabidce { get; set; } // Záporná pozice = počítáno od konce
        public bool Kliknuto { get; set; }

        public PolozkaNabidky(short poziceVNabidce, TypPolozkyNabidky typPolozky)
        {
            PoziceVNabidce = poziceVNabidce;
            TypPolozky = typPolozky;

            UhelKorkceObrazku = 0;
            Okraje = 8;

            // Grafika netextových typů
            if (TypPolozky == TypPolozkyNabidky.VezKulomet)
                Dlazdice = new[] {
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Zakladna_1, 0.90f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Kulomet_1, 0.91f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.VezRaketa)
                Dlazdice = new[] {
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Zakladna_2, 0.90f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Raketa_1_Stred, 0.92f, false),
                    new DlazdiceUrceni(ZakladniDlazdice.Vez_Raketa_1_Vrsek, 0.95f, false),
                };
            else if (TypPolozky == TypPolozkyNabidky.Vymazat)
            {
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Kos, 0.90f, false), };
                Barva = Color.Silver;
                Okraje = 24;
            }
            else if (TypPolozky == TypPolozkyNabidky.Upgrade)
            {
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Upgrade, 0.90f, false), };
                Okraje = 24;
            }
            else if (TypPolozky == TypPolozkyNabidky.Pauza)
            {
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Pauza, 0.90f, false), };
                Okraje = 24;

            }
            else if (TypPolozky == TypPolozkyNabidky.Text)
            {
                Meritko = MaxMeritkoTextu; // Nastavení měřítka textu a výběrovému políčku
                Text = " ";
            }
        }

        internal Rectangle Vyrez;
        public virtual void Update(float sekundOdMinule, Vector2 klik, Rectangle vyrez)
        {
            this.Vyrez = vyrez;
            Kliknuto = false;
            if (Skryta) return;

            Update(sekundOdMinule);

            //// Pozice - levý horní roh dlaždice
            //var pozice = new Point((PoziceVNabidce >= 0 ? PoziceVNabidce :
            //                    Zdroje.Aktualni.Level.Mapa.Sloupcu + PoziceVNabidce) * Zdroje.VelikostDlazdice,
            //                    Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice);

            var pozice = new Point((PoziceVNabidce >= 0 ? PoziceVNabidce :
                                OvladaciPanel.SirkaNabidky + PoziceVNabidce) * Zdroje.VelikostDlazdice + vyrez.X,
                                vyrez.Y);

            if (klik != Vector2.Zero)
                if (new Rectangle(pozice.X, pozice.Y, Zdroje.VelikostDlazdice, Zdroje.VelikostDlazdice).Contains(klik))
                    Kliknuto = true;
        }

        public override void Update(float sekundOdMinule)
        {
            if (Skryta) return;
           
            base.Update(sekundOdMinule);

            if (TypPolozky == TypPolozkyNabidky.Text)
            {
                var rozmery = Zdroje.Obsah.Pismo.MeasureString(Text);
                Stred = new Vector2(ZarovnatTextHorizontalneNaStred ? rozmery.X * 0.5f : (PoziceVNabidce < 0 ? rozmery.X : 0), 
                                    rozmery.Y * 0.5f);
                
                float x;                
                if (ZarovnatTextHorizontalneNaStred) // Zarovnat na střed
                    x = PoziceVNabidce * Zdroje.VelikostDlazdice + Okraje.Vlevo 
                        + (SirkaTextu * Zdroje.VelikostDlazdice - Okraje.Horizontalne) * 0.5f;
                else if (PoziceVNabidce < 0) // Zarovnat doprava
                    x = (OvladaciPanel.SirkaNabidky + PoziceVNabidce + 1) * Zdroje.VelikostDlazdice - Okraje.Vpravo;
                else // Zarovnat doleva
                    x = PoziceVNabidce * Zdroje.VelikostDlazdice + Okraje.Vlevo;

                Pozice = new Vector2(x + Vyrez.X,
                                     //Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice 
                                     Vyrez.Y + Okraje.Nahore 
                                     + (Zdroje.VelikostDlazdice - Okraje.Vertikalne) * 0.5f);

                // Zmenšení měřítka (je-li to za potřebí), aby se text vešel do zadaného počtu dlaždic na šířku
                var meze = new Vector2(SirkaTextu * Zdroje.VelikostDlazdice - Okraje.Horizontalne,
                                       Zdroje.VelikostDlazdice - Okraje.Vertikalne);
                var meritko = meze / rozmery; // new Vector2(meze.X / rozmery.X, meze.Y / rozmery.Y);

                Meritko = Math.Min(Math.Min(meritko.X, meritko.Y), MaxMeritkoTextu);
            }
            else
            {
                // Pozice - levý horní roh dlaždice
                var pozice = new Point((PoziceVNabidce >= 0 ? PoziceVNabidce :
                                    OvladaciPanel.SirkaNabidky + PoziceVNabidce) * Zdroje.VelikostDlazdice + Vyrez.X, // * Zdroje.VelikostDlazdice,
                                    Vyrez.Y);

                // Pozice netextových položek nabídky - střed dlaždice
                Pozice = new Vector2(pozice.X + 0.5f * (Zdroje.VelikostDlazdice - Okraje.Horizontalne) + Okraje.Vlevo,
                                     pozice.Y + 0.5f * (Zdroje.VelikostDlazdice - Okraje.Vertikalne) + Okraje.Nahore);

                // Výpočet měřítka vzhledem k okrajům
                Meritko = (Zdroje.VelikostDlazdice
                           - Math.Max(Okraje.Horizontalne, Okraje.Vertikalne))
                           / (float)Zdroje.VelikostDlazdice;
            }
        }



        public override void Draw(SpriteBatch sb)
        {
            if (Skryta) return;
            if (TypPolozky == TypPolozkyNabidky.Text)
            {
                sb.DrawString(Zdroje.Obsah.Pismo, Text ?? "", Pozice, Barva,
                    0, Stred, Meritko, SpriteEffects.None, 0.9f);
            }
            else
                base.Draw(sb);
        }

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
        }


        public void NastavTextCeny(float cena, bool pridatMenu = true, bool pridatZnamenko = true)
        {
            Text = (pridatZnamenko ? (cena < 0 ? "-" : "+") : "") +
                   (pridatMenu ? "$" : "") + 
                   Math.Abs(cena).ToString();
            Barva = (cena < 0 && Zdroje.Aktualni.Level.Konto + cena < 0) ? Color.Red : Color.Lime;
        }
    }

}
