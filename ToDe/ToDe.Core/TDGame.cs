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
        List<MizejiciObjekt> mizejiciObjekty;
        List<PolozkaNabidky> nabidka;
        List<Vez> veze;
        List<Raketa> rakety;
        List<Exploze> exploze;
        protected override void Initialize()
        {
            nepratele = new List<Nepritel>();
            mizejiciObjekty = new List<MizejiciObjekt>();
            nabidka = new List<PolozkaNabidky>();
            veze = new List<Vez>();
            rakety = new List<Raketa>();
            exploze = new List<Exploze>();

            base.Initialize();
        }

        float zdravi;
        PolozkaNabidky textPocetVeziKluomet, textPocetVeziRaketa, textZivotu, textFinance;
        Zdroje aktualniMapa;
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Načtení zdrojů
            Zdroje.Obsah = new Obsah() {
                Pismo = Content.Load<SpriteFont>(@"Fonts/Pismo"),
                ZvukKulomet = new Zvuk(Content.Load<SoundEffect>(@"Sounds/vez_kulomet"), 2),
                ZvukRaketaStart = new Zvuk(Content.Load<SoundEffect>(@"Sounds/raketa_start"), 3, 0.75f),
                ZvukRaketaDopad = new Zvuk(Content.Load<SoundEffect>(@"Sounds/raketa_dopad"), 3, 0.5f),
                ZvukKonecVyhra = new Zvuk(Content.Load<SoundEffect>(@"Sounds/konec_vyhra"), 1),
                ZvukKonecProhra = new Zvuk(Content.Load<SoundEffect>(@"Sounds/konec_prohra"), 1),
            };
            Zdroje.Obsah.Zakladni.Grafika = Content.Load<Texture2D>(@"Sprites/textura-vyber");
            Zdroje.Obsah.Exploze.Grafika = Content.Load<Texture2D>(@"Sprites/exploze");
            Zdroje.Obsah.Ukazatel.Grafika = Content.Load<Texture2D>(@"Sprites/ukazatel");

            SpustitHru();
           
            // Sestavení panelu nabídky
            nabidka.Add(new PolozkaNabidky(0, TypPolozkyNabidky.VezKulomet));
            nabidka.Add(textPocetVeziKluomet = new PolozkaNabidky(1, TypPolozkyNabidky.Text));
            nabidka.Add(new PolozkaNabidky(3, TypPolozkyNabidky.VezRaketa));
            nabidka.Add(textPocetVeziRaketa = new PolozkaNabidky(4, TypPolozkyNabidky.Text));
            nabidka.Add(new PolozkaNabidky(-3, TypPolozkyNabidky.Pauza));
            nabidka.Add(textFinance = new PolozkaNabidky(-4, TypPolozkyNabidky.Text));
            nabidka.Add(textZivotu = new PolozkaNabidky(-1, TypPolozkyNabidky.Text));

            NastavTexty(true);

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
            aktualniMapa = Zdroje.NactiLevel(ref Zdroje.CisloLevelu);

            // Nastavení hry
            zdravi = 1;

            jeKonecHry = false;
            pauza = false;
        }

        void NastavTexty(bool novyLevel = false)
        {
            // Texty do nabídky
            textFinance.Text = "$" + (Math.Floor(Zdroje.Aktualni.Level.Konto / 10.0)*10).ToString();

            int zbytekZivota = (int)Math.Round(zdravi * 100);
            if (zdravi < 0)
                zbytekZivota = 0;
            else if (zbytekZivota > 100)
                zbytekZivota = 100;
            else if (zbytekZivota == 0 && zdravi > 0)
                zbytekZivota = 1;

            textZivotu.Text = zbytekZivota + "%";

            if (novyLevel)
            {
                PolozkaNabidky.Vyber.Viditelny = false;
                textPocetVeziKluomet.Text = "$" + KonfiguraceVezKulomet.VychoziParametry.Cena.ToString();
                textPocetVeziRaketa.Text = "$" + KonfiguraceVezRaketa.VychoziParametry.Cena.ToString();
            }
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
            // Kliknutí
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

            // Přepočet časů na sekundy
            float celkovyHerniCas = (float)gameTime.TotalGameTime.TotalSeconds;
            float casOdMinule = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Konec hry - čekání na klik pro zahájení nové hry
            if (zdravi <= 0) // Jsme mrtví...
            {
                // První detekce, že hra skončila naší prohrou
                if (!jeKonecHry)
                {
                    jeKonecHry = true;
                    Zdroje.CisloLevelu = 1; // Až kliknem a zahájíme novou hru, tak začne od prvního kola
                    Zdroje.Obsah.ZvukKonecProhra.HrajZvuk(celkovyHerniCas);
                }

                // Kliknutí při konci hry = zahaj novou hru
                if (poziceKliknuti != Vector2.Zero)
                {
                    SpustitHru();
                    NastavTexty(true);
                    AkutalizovatPlanUtoku(gameTime.TotalGameTime.TotalSeconds);
                }
                base.Update(gameTime);
                return;
            }
            // Konec hry výhrou
            else if (nepratele.Count == 0 && Zdroje.Aktualni.Level.PlanPosilaniVln.Count == 0) 
            {
                // První detekce, že hra skončila naší výhrou
                if (!jeKonecHry)
                {
                    jeKonecHry = true;
                    Zdroje.CisloLevelu++;
                    Zdroje.Obsah.ZvukKonecVyhra.HrajZvuk(celkovyHerniCas);
                }
                // Kliknutí na oznámení výhry = jde se na další kolo
                if (poziceKliknuti != Vector2.Zero)
                {
                    SpustitHru();
                    NastavTexty(true);
                    AkutalizovatPlanUtoku(gameTime.TotalGameTime.TotalSeconds);
                }
            }

            // Aktualizace nabídky (ještě před pauzou, aby se dala odpauzovat)
            nabidka.ForEach(x => x.Update(casOdMinule, poziceKliknuti));

            // Pauzování (zapnout/vyponout pauzu)
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


            if (!pauza) // Další objekty se aktualizují pouze pokud není zapnutá pauza
            {
                // Navýšení konta o finance, které nám přibývají za sekundu
                Zdroje.Aktualni.Level.Konto += Zdroje.Aktualni.Level.RychlostBohatnuti * casOdMinule;

                // Umístění věže na mapě
                if (poziceKliknuti != Vector2.Zero &&
                    poziceKliknuti.X < Zdroje.Aktualni.Level.Mapa.Sloupcu * Zdroje.VelikostDlazdice &&
                    poziceKliknuti.Y < Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice) // Kliknuto do prostoru mapy
                {
                    Point souradniceNaMape = new Point(
                            (int)poziceKliknuti.X / Zdroje.VelikostDlazdice,
                            (int)poziceKliknuti.Y / Zdroje.VelikostDlazdice
                        );
                    if (Zdroje.Aktualni.Level.Mapa.Pozadi[souradniceNaMape.Y, souradniceNaMape.X] == TypDlazdice.Plocha &&
                        !veze.Any(x => x.SouradniceNaMape == souradniceNaMape)) // Kliklo se mimo silnici a není tam už věž
                    {
                        var typ = nabidka.FirstOrDefault(x => x.Oznacen)?.TypPolozky; // Vybraný typ věže ve spodní nabídce

                        // Umístnění kulometné věže
                        if (typ == TypPolozkyNabidky.VezKulomet &&  
                            KonfiguraceVezKulomet.VychoziParametry.Cena <= Zdroje.Aktualni.Level.Konto)
                        {
                            veze.Add((new VezKulomet()).UmistiVez(souradniceNaMape));
                            Zdroje.Aktualni.Level.Konto -= KonfiguraceVezKulomet.VychoziParametry.Cena;
                        }
                        // Umístění raketové věže
                        else if (typ == TypPolozkyNabidky.VezRaketa && 
                                 KonfiguraceVezRaketa.VychoziParametry.Cena <= Zdroje.Aktualni.Level.Konto)
                        {
                            veze.Add((new VezRaketa()).UmistiVez(souradniceNaMape));
                            Zdroje.Aktualni.Level.Konto -= KonfiguraceVezRaketa.VychoziParametry.Cena;
                        }
                    }
                }

                // Aktualizace ostatních herních objektů
                nepratele.ForEach(x => x.Update(casOdMinule));
                mizejiciObjekty.ForEach(x => x.Update(casOdMinule));

                // Otáčení (update) věží (řeší se zvlášť)
                nepratele = nepratele.OrderByDescending(x => x.VzdalenostNaCeste).ToList(); // Seřazení seznamu nepřátel, aby ti nejvíce vepředu byli první
                foreach (var vez in veze)
                {
                    vez.Cil = nepratele.FirstOrDefault(x => Vector2.Distance(vez.Pozice, x.Pozice) < vez.DosahStrelby); // Vybrat cíl věži, na který dostane a je nejvíce vepředu
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
                    // Dopad (výbuch) rakety
                    if (raketa.Dopad)
                    {
                        Zdroje.Obsah.ZvukRaketaDopad.HrajZvuk(celkovyHerniCas);
                        mizejiciObjekty.Add(new Dira() { Pozice = raketa.Pozice }); // Vykreslit díru na cestě
                        exploze.Add(new Exploze(raketa));
                        // Ubrání života nepřátelům v okolí
                        var cile = nepratele
                            .Select(x => new { Vzdalenost = Vector2.Distance(raketa.Pozice, x.Pozice), Nepritel = x })
                            .Where(x => x.Vzdalenost < raketa.DosahExploze);
                        foreach (var cil in cile)
                            cil.Nepritel.Zdravi -= raketa.SilaStrely * (raketa.DosahExploze - cil.Vzdalenost) / raketa.DosahExploze;
                    }
                }
                exploze.ForEach(x => x.Update(casOdMinule)); // Aktualizace explozí

                // Vypouštění nepřátel (vlny)
                if (Zdroje.Aktualni.Level.PlanPosilaniVln.Count > 0)
                {
                    if (Zdroje.Aktualni.Level.PlanPosilaniVln[0].Cas <= celkovyHerniCas)
                    {
                        nepratele.Add(new Nepritel(Zdroje.Aktualni.Level.PlanPosilaniVln[0].Jednotka));
                        Zdroje.Aktualni.Level.PlanPosilaniVln.RemoveAt(0); // Už byl vyslán, smazat z plánu
                    }
                }

                // Kontrola Vojáků, kteří došli do cíle
                foreach (var nepritel in nepratele.Where(x => x.DosahlCile))
                {
                    zdravi = Math.Max(zdravi - nepritel.SilaUtoku * nepritel.ProcentoZdravi, 0); // Ubrat hráči zdraví
                }
                
                // Aktualizace textů v nabídce
                NastavTexty();                


                // Odstraňování smazaných objektů ze seznamů
                nepratele.RemoveAll(x => x.Smazat);
                mizejiciObjekty.RemoveAll(x => x.Smazat);
                rakety.RemoveAll(x => x.Smazat);
                exploze.RemoveAll(x => x.Smazat);
                nabidka.ForEach(x => x.Update(casOdMinule));
            }
            base.Update(gameTime);
        }

        float globalniMeritko = 1; // Uložení měřítka grafiky, pro přepočet pozice kliknutí
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // Barva podkladu na pozadí

            // Matice pro měřítko (zoom) vykreslování, aby se vše vešlo do okna
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)(aktualniMapa.Level.Mapa.Sloupcu * Zdroje.VelikostDlazdice);
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)((aktualniMapa.Level.Mapa.Radku + 1) * Zdroje.VelikostDlazdice);
            globalniMeritko = MathHelper.Min(scaleX, scaleY); // Použijeme měřítko, aby se vše vždy vešlo do obrazovky
            var screenScale = new Vector3(new Vector2(globalniMeritko), 1.0f);
            var scaleMatrix = Matrix.CreateScale(screenScale);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, transformMatrix: scaleMatrix); // Začátek vykreslování

            // Vykreslení pozadí mapy
            for (int i = 0; i < aktualniMapa.Level.Mapa.Radku; i++)
                for (int j = 0; j < aktualniMapa.Level.Mapa.Sloupcu; j++)
                    spriteBatch.Kresli(aktualniMapa.Level.Mapa.CilDlazdice(i, j), 
                                       aktualniMapa.Level.Mapa.MezeDlazdice(i, j), Vector2.Zero);

            // Vykreslování seznamů herních objektů
            nepratele.ForEach(x => x.Draw(spriteBatch));
            mizejiciObjekty.ForEach(x => x.Draw(spriteBatch));
            veze.ForEach(x => x.Draw(spriteBatch));
            rakety.ForEach(x => x.Draw(spriteBatch));
            exploze.ForEach(x => x.Draw(spriteBatch));
            nabidka.ForEach(x => x.Draw(spriteBatch));
            PolozkaNabidky.Vyber.Draw(spriteBatch); // Vykreslení označovacího rámečku v nabídce

            // Game Over
            if (zdravi <= 0) // Prohra
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
