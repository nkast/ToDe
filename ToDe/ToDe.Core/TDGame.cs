using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToDe
{
    public class TDGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public TDGame()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
        }

        List<Nepritel> nepratele;
        List<MizejiciObraz> mizejiciObjekty;
        List<PolozkaNabidky> nabidka;
        List<Vez> veze;
        List<Raketa> rakety;
        List<Exploze> exploze;
        protected override void Initialize()
        {
            nepratele = new List<Nepritel>();
            mizejiciObjekty = new List<MizejiciObraz>();
            nabidka = new List<PolozkaNabidky>();
            veze = new List<Vez>();
            rakety = new List<Raketa>();
            exploze = new List<Exploze>();

            base.Initialize();
        }

        ushort pocetVeziKluomet, pocetVeziRaketa;
        float zdravi;
        PolozkaNabidky textPocetVeziKluomet, textPocetVeziRaketa, textZivotu;
        Zdroje aktualniMapa;
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Načtení zdrojů
            Zdroje.Obsah = new Obsah() {
                Zakladni = Content.Load<Texture2D>(@"Sprites/Basic"),
                Exploze = Content.Load<Texture2D>(@"Sprites/exploze"),
                Pismo = Content.Load<SpriteFont>(@"Fonts/Pismo"),
                ZvukKulomet = new Zvuk(Content.Load<SoundEffect>(@"Sounds/vez_kulomet"), 2),
                ZvukRaketaStart = new Zvuk(Content.Load<SoundEffect>(@"Sounds/raketa_start"), 3, 0.75f),
                ZvukRaketaDopad = new Zvuk(Content.Load<SoundEffect>(@"Sounds/raketa_dopad"), 3, 0.5f),
                ZvukKonecVyhra = new Zvuk(Content.Load<SoundEffect>(@"Sounds/konec_vyhra"), 1),
                ZvukKonecProhra = new Zvuk(Content.Load<SoundEffect>(@"Sounds/konec_prohra"), 1),
            };

            SpustitHru();
           
            // Sestavení panelu nabídky
            nabidka.Add(new PolozkaNabidky(0, TypPolozkyNabidky.VezKulomet));
            nabidka.Add(textPocetVeziKluomet = new PolozkaNabidky(1, TypPolozkyNabidky.Text));
            nabidka.Add(new PolozkaNabidky(3, TypPolozkyNabidky.VezRaketa));
            nabidka.Add(textPocetVeziRaketa = new PolozkaNabidky(4, TypPolozkyNabidky.Text));
            nabidka.Add(new PolozkaNabidky(-3, TypPolozkyNabidky.Pauza));
            nabidka.Add(textZivotu = new PolozkaNabidky(-1, TypPolozkyNabidky.Text));

            NastavTexty();

            base.LoadContent();
        }

        private void SpustitHru()
        {
            nepratele.Clear();
            mizejiciObjekty.Clear();
            //nabidka.Clear();
            veze.Clear();
            rakety.Clear();
            exploze.Clear();

            // Načtení levelu
            aktualniMapa = Zdroje.NactiLevel(Zdroje.CisloLevelu);

            // Nastavení hry
            zdravi = 1;
            pocetVeziKluomet = (ushort)(aktualniMapa.Level.Veze.FirstOrDefault(x => x.Typ == TypVeze.Kulomet)?.Pocet ?? 0);
            pocetVeziRaketa = (ushort)(aktualniMapa.Level.Veze.FirstOrDefault(x => x.Typ == TypVeze.Raketa)?.Pocet ?? 0);

            jeKonecHry = false;
            pauza = false;
        }

        void NastavTexty()
        {
            // Texty do nabídky
            textPocetVeziKluomet.Text = pocetVeziKluomet.ToString();
            textPocetVeziRaketa.Text = pocetVeziRaketa.ToString();
            textZivotu.Text = (zdravi * 100) + "%";
            PolozkaNabidky.Vyber.Viditelny = false;
        }

        void AkutalizovatPlanUtoku(double oKolik)
        {
            Zdroje.Aktualni.Level.PlanPosilaniVln.ForEach(x => x.Cas = x.Cas + (float)oKolik);
        }

        bool byloKliknutoMinule = false;
        bool pauza = false, jeKonecHry = false;
        double casPriPauznuti;
        protected override void Update(GameTime gameTime)
        {
            // Načtení kliknutí
            Vector2 poziceKliknuti = Vector2.Zero;
            // Dotyk
            poziceKliknuti = TouchPanel.GetState().FirstOrDefault(tl => tl.State == TouchLocationState.Pressed).Position;
            // Myš
            var ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
                poziceKliknuti = new Vector2(ms.X, ms.Y);
            poziceKliknuti /= globalniMeritko;
            // Jde o nový klik?
            bool kliknutiZahajeno = !byloKliknutoMinule && poziceKliknuti != Vector2.Zero;
            if (byloKliknutoMinule && poziceKliknuti != Vector2.Zero)
            {
                byloKliknutoMinule = true;
                poziceKliknuti = Vector2.Zero;
            }
            else
                byloKliknutoMinule = poziceKliknuti != Vector2.Zero;

            float celkovyHerniCas = (float)gameTime.TotalGameTime.TotalSeconds;
            float casOdMinule = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Konec hry - čekání na klik pro zahájení nové hry
            if (zdravi <= 0)
            {
                if (!jeKonecHry)
                {
                    jeKonecHry = true;
                    Zdroje.CisloLevelu = 1;
                    Zdroje.Obsah.ZvukKonecProhra.HrajZvuk(celkovyHerniCas);
                }

                if (poziceKliknuti != Vector2.Zero)
                {
                    SpustitHru();
                    //ResetElapsedTime();
                    NastavTexty();
                    AkutalizovatPlanUtoku(gameTime.TotalGameTime.TotalSeconds);
                }
                base.Update(gameTime);
                return;
            } 
            else if (nepratele.Count == 0 && Zdroje.Aktualni.Level.PlanPosilaniVln.Count == 0) // Konec hry výhrou
            {
                if (!jeKonecHry)
                {
                    jeKonecHry = true;
                    Zdroje.CisloLevelu++;
                    Zdroje.Obsah.ZvukKonecVyhra.HrajZvuk(celkovyHerniCas);
                }
                if (poziceKliknuti != Vector2.Zero)
                {
                    SpustitHru();
                    NastavTexty();
                    AkutalizovatPlanUtoku(gameTime.TotalGameTime.TotalSeconds);
                }
            }

            nabidka.ForEach(x => x.Update(casOdMinule, poziceKliknuti));

            // Pauzování
            var oznacenaPolozkaNabidky = nabidka.FirstOrDefault(x => x.Oznacen)?.TypPolozky;
            if (oznacenaPolozkaNabidky == TypPolozkyNabidky.Pauza && !pauza)
            {
                pauza = true;
                casPriPauznuti = gameTime.TotalGameTime.TotalSeconds;
            } else if (oznacenaPolozkaNabidky != TypPolozkyNabidky.Pauza && pauza)
            { 
                pauza = false;
                // Přepočítat plán útoku
                AkutalizovatPlanUtoku(gameTime.TotalGameTime.TotalSeconds - casPriPauznuti);
            }


            if (!pauza)
            {
                // Umístění věže na mapě
                if (poziceKliknuti != Vector2.Zero &&
                    poziceKliknuti.X < Zdroje.Aktualni.Level.Mapa.Sloupcu * Zdroje.VelikostDlazdice &&
                    poziceKliknuti.Y < Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice)
                {
                    Point souradniceNaMape = new Point(
                            (int)poziceKliknuti.X / Zdroje.VelikostDlazdice,
                            (int)poziceKliknuti.Y / Zdroje.VelikostDlazdice
                        );
                    if (Zdroje.Aktualni.Level.Mapa.Pozadi[souradniceNaMape.Y, souradniceNaMape.X] == TypDlazdice.Ground &&
                        !veze.Any(x => x.SouradniceNaMape == souradniceNaMape))
                    {
                        var typ = nabidka.FirstOrDefault(x => x.Oznacen)?.TypPolozky;

                        if (typ == TypPolozkyNabidky.VezKulomet && pocetVeziKluomet > 0)
                        {
                            veze.Add((new VezKulomet()).UmistiVez(souradniceNaMape));
                            pocetVeziKluomet--;
                            textPocetVeziKluomet.Text = pocetVeziKluomet.ToString();
                        }
                        else if (typ == TypPolozkyNabidky.VezRaketa && pocetVeziRaketa > 0)
                        {
                            veze.Add((new VezRaketa()).UmistiVez(souradniceNaMape));
                            pocetVeziRaketa--;
                            textPocetVeziRaketa.Text = pocetVeziRaketa.ToString();
                        }
                    }
                }

                // Aktualizace herních objektů
                nepratele.ForEach(x => x.Update(casOdMinule));
                mizejiciObjekty.ForEach(x => x.Update(casOdMinule));

                // Otáčení (update) věží
                nepratele = nepratele.OrderByDescending(x => x.VzdalenostNaCeste).ToList();
                foreach (var vez in veze)
                {
                    vez.Cil = nepratele.FirstOrDefault(x => Vector2.Distance(vez.Pozice, x.Pozice) < vez.DosahStrelby);
                    vez.Update(casOdMinule);
                    // Výstřel
                    if (vez.Strelba)
                    {
                        if (vez is VezKulomet)
                        {
                            Zdroje.Obsah.ZvukKulomet.HrajZvuk(celkovyHerniCas);
                            mizejiciObjekty.Add(new PlamenStrelby((VezKulomet)vez));
                        }
                        else if (vez is VezRaketa)
                        {
                            Zdroje.Obsah.ZvukRaketaStart.HrajZvuk(celkovyHerniCas);
                            rakety.Add(new Raketa((VezRaketa)vez));
                        }
                    }
                }

                // Rakety
                foreach (var raketa in rakety)
                {
                    raketa.Update(casOdMinule);
                    if (raketa.Dopad)
                    {
                        Zdroje.Obsah.ZvukRaketaDopad.HrajZvuk(celkovyHerniCas);
                        mizejiciObjekty.Add(new Dira() { Pozice = raketa.Pozice });
                        exploze.Add(new Exploze(raketa));
                        // Ubrání života nepřátelům v okolí
                        var cile = nepratele
                            .Select(x => new { Vzdalenost = Vector2.Distance(raketa.Pozice, x.Pozice), Nepritel = x })
                            .Where(x => x.Vzdalenost < raketa.DosahExploze);
                        foreach (var cil in cile)
                            cil.Nepritel.Zdravi -= raketa.SilaStrely * (raketa.DosahExploze - cil.Vzdalenost) / raketa.DosahExploze;
                    }
                }
                exploze.ForEach(x => x.Update(casOdMinule));

                // Vypouštění nepřátel
                if (Zdroje.Aktualni.Level.PlanPosilaniVln.Count > 0)
                {
                    if (Zdroje.Aktualni.Level.PlanPosilaniVln[0].Cas <= celkovyHerniCas)
                    {
                        nepratele.Add(new Nepritel(Zdroje.Aktualni.Level.PlanPosilaniVln[0].Jednotka));
                        Zdroje.Aktualni.Level.PlanPosilaniVln.RemoveAt(0);
                    }
                }

                // Kontrola Vojáků, kteří došli do cíle
                foreach (var nepritel in nepratele.Where(x => x.DosahlCile))
                {
                    zdravi = Math.Max(zdravi - nepritel.SilaUtoku, 0);
                }
                textZivotu.Text = Math.Round(zdravi * 100) + "%";


                // Odstraňování smazaných objektů ze seznamů
                nepratele.RemoveAll(x => x.Smazat);
                mizejiciObjekty.RemoveAll(x => x.Smazat);
                rakety.RemoveAll(x => x.Smazat);
                exploze.RemoveAll(x => x.Smazat);
            }
            base.Update(gameTime);
        }

        float globalniMeritko = 1;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // Barva podkladu na pozadí

            // Matice pro měřítko (zoom) vykreslování, aby se vše vešlo do okna
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)(aktualniMapa.Level.Mapa.Sloupcu * Zdroje.VelikostDlazdice);
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)((aktualniMapa.Level.Mapa.Radku + 1) * Zdroje.VelikostDlazdice);
            globalniMeritko = MathHelper.Min(scaleX, scaleY);
            var screenScale = new Vector3(new Vector2(globalniMeritko), 1.0f);
            var scaleMatrix = Matrix.CreateScale(screenScale);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, transformMatrix: scaleMatrix); // Začátek vykreslování


            // Vykreslení pozadí mapy
            for (int i = 0; i < aktualniMapa.Level.Mapa.Radku; i++)
                for (int j = 0; j < aktualniMapa.Level.Mapa.Sloupcu; j++)
                {
                    spriteBatch.Kresli(aktualniMapa.Level.Mapa.CilDlazdice(i, j), aktualniMapa.Level.Mapa.MezeDlazdice(i, j), Vector2.Zero);
                }

            // Vykreslování seznamů
            nepratele.ForEach(x => x.Draw(spriteBatch));
            mizejiciObjekty.ForEach(x => x.Draw(spriteBatch));
            veze.ForEach(x => x.Draw(spriteBatch));
            rakety.ForEach(x => x.Draw(spriteBatch));
            exploze.ForEach(x => x.Draw(spriteBatch));
            nabidka.ForEach(x => x.Draw(spriteBatch));
            PolozkaNabidky.Vyber.Draw(spriteBatch); // Vykreslení označovacího rámečku v nabídce
            //nabidka.FirstOrDefault().DrawVyber(spriteBatch); // Vykreslení označovacího rámečku v nabídce

            // Game Over
            if (zdravi <= 0)
            {
                spriteBatch.KresliTextDoprostred("GAME OVER");
            } 
            else if (jeKonecHry) // Výhra
            {
                spriteBatch.KresliTextDoprostred("VICTORY");
            }


            // Konec vykreslování
            spriteBatch.End(); 
            base.Draw(gameTime);
        }
    }
}
