using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    public enum TypPolozkyNabidky
    {
        VezKulomet,
        VezRaketa,
        Pauza,
        Text,
        Vyber, // Obdélníček značící výběr položky
    }

    internal class PolozkaNabidky : HerniObjekt
    {
        public TypPolozkyNabidky TypPolozky { get; private set; }
        public string Text { get; set; } // Pouze pro typ text
        public short PoziceVNabidce { get; private set; } // Záporná pozice = počítáno od konce

        public static PolozkaNabidky Vyber { get; private set; }
        public bool Oznacen { get => Vyber?.Viditelny == true && Vyber?.PoziceVNabidce == PoziceVNabidce; }
        public bool Viditelny { get; set; } = false; // Platí jen pro Vyber

        public PolozkaNabidky(short poziceVNabidce, TypPolozkyNabidky typPolozky)
        {
            PoziceVNabidce = poziceVNabidce;
            TypPolozky = typPolozky;

            UhelKorkceObrazku = 0;

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
            else if (TypPolozky == TypPolozkyNabidky.Pauza)
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Pauza, 0.0f, false), };
            else if (TypPolozky == TypPolozkyNabidky.Vyber)
            {
                Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Vyber, 0.9f, false) };
            }

            // Nastavení měřítka textu a výběrovému políčku
            if (TypPolozky == TypPolozkyNabidky.Text)
                Meritko = 0.5f;
            else if (TypPolozky != TypPolozkyNabidky.Vyber)
                Meritko = 0.75f;

            // Výběrové označovátko
            if (Vyber == null && TypPolozky != TypPolozkyNabidky.Vyber)
            {
                Vyber = new PolozkaNabidky(-1, TypPolozkyNabidky.Vyber);
            }
        }

        public void Update(float elapsedSeconds, Vector2 klik)
        {
            Update(elapsedSeconds);

            if (TypPolozky == TypPolozkyNabidky.Text) return; // Text se řeší v předchozím příkazu

            // Pozice - levý horní roh dlaždice
            var pozice = new Point((PoziceVNabidce >= 0 ? PoziceVNabidce :
                                Zdroje.Aktualni.Level.Mapa.Sloupcu + PoziceVNabidce) * Zdroje.VelikostDlazdice,
                                Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice);

            // Pozice netextových položek nabídky - střed dlaždice
            Pozice = new Vector2(pozice.X + 0.5f * Zdroje.VelikostDlazdice, pozice.Y + 0.5f * Zdroje.VelikostDlazdice);

            if (klik != Vector2.Zero)
            {
                if (new Rectangle(pozice.X, pozice.Y, Zdroje.VelikostDlazdice, Zdroje.VelikostDlazdice).Contains(klik))
                {
                    if (Vyber.PoziceVNabidce == PoziceVNabidce)
                        Vyber.Viditelny = !Vyber.Viditelny;
                    else
                    {
                        Vyber.Viditelny = true;
                        Vyber.PoziceVNabidce = PoziceVNabidce;
                        Vyber.Pozice = Pozice;
                    }
                }
            }

        }

        public override void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);

            if (TypPolozky != TypPolozkyNabidky.Text) return;

            var rozmery = Zdroje.Obsah.Pismo.MeasureString(Text);
            Stred = new Vector2(PoziceVNabidce < 0 ? rozmery.X : 0, rozmery.Y * 0.5f);
            Pozice = new Vector2((PoziceVNabidce < 0
                                    ? Zdroje.Aktualni.Level.Mapa.Sloupcu + PoziceVNabidce + 1 - 0.1f
                                    : PoziceVNabidce + 0.1f)
                                    * Zdroje.VelikostDlazdice,
                                (Zdroje.Aktualni.Level.Mapa.Radku + 0.5f) * Zdroje.VelikostDlazdice);
        }


        public override void Draw(SpriteBatch sb)
        {
            if (TypPolozky == TypPolozkyNabidky.Vyber && !Viditelny)
                return;

            if (TypPolozky == TypPolozkyNabidky.Text)
            {
                sb.DrawString(Zdroje.Obsah.Pismo, Text, Pozice, Color.White,
                    UhelOtoceni, Stred, Meritko, SpriteEffects.None, 0);
            }
            else
                base.Draw(sb);
        }
    }

}
