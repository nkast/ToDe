using Microsoft.Xna.Framework;
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
            Zdroje.Textury = new Textury() {
                Zakladni = Content.Load<Texture2D>(@"Sprites/Basic"),
                Exploze = Content.Load<Texture2D>(@"Sprites/exploze"),
                Pismo = Content.Load<SpriteFont>(@"Fonts/Pismo"),
            };

            // Načtení levelu
            aktualniMapa = Zdroje.NactiLevel(1);

            // Nastavení hry
            zdravi = 1;
            pocetVeziKluomet = 10;
            pocetVeziRaketa = 5;

            // Sestavení panelu nabídky
            nabidka.Add(new PolozkaNabidky(0, TypPolozkyNabidky.VezKulomet));
            nabidka.Add(textPocetVeziKluomet = new PolozkaNabidky(1, TypPolozkyNabidky.Text) { Text = pocetVeziKluomet.ToString() });
            nabidka.Add(new PolozkaNabidky(3, TypPolozkyNabidky.VezRaketa));
            nabidka.Add(textPocetVeziRaketa = new PolozkaNabidky(4, TypPolozkyNabidky.Text) { Text = pocetVeziRaketa.ToString() });
            nabidka.Add(textZivotu = new PolozkaNabidky(-1, TypPolozkyNabidky.Text) { Text = (zdravi*100) + "%" });

            // Testovací objekty
            //nepratele.Add(new Nepritel());          


            base.LoadContent();
        }

        bool byloKliknutoMinule = false;
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
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            nepratele.ForEach(x => x.Update(seconds));
            mizejiciObjekty.ForEach(x => x.Update(seconds));            
            nabidka.ForEach(x => x.Update(seconds, poziceKliknuti));

            // Otáčení (update) věží
            nepratele = nepratele.OrderByDescending(x => x.VzdalenostNaCeste).ToList();
            foreach (var vez in veze)
            {
                vez.Cil = nepratele.FirstOrDefault(x => Vector2.Distance(vez.Pozice, x.Pozice) < vez.DosahStrelby);
                vez.Update(seconds);
                // Výstřel
                if (vez.Strelba)
                {
                    if (vez is VezKulomet)
                    {
                        mizejiciObjekty.Add(new PlamenStrelby((VezKulomet)vez));
                    }
                    else if (vez is VezRaketa)
                    {
                        rakety.Add(new Raketa((VezRaketa)vez));
                    }
                }
            }

            // Rakety
            foreach (var raketa in rakety)
            {
                raketa.Update(seconds);
                if (raketa.Dopad)
                {
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
            exploze.ForEach(x => x.Update(seconds));

            // Vypouštění nepřátel
            if (Zdroje.Aktualni.Level.PlanPosilaniVln.Count > 0)
            {
                if (Zdroje.Aktualni.Level.PlanPosilaniVln[0].Cas <= gameTime.TotalGameTime.TotalSeconds)
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
            nabidka.FirstOrDefault().DrawVyber(spriteBatch); // Vykreslení označovacího rámečku v nabídce

            // Konec vykreslování
            spriteBatch.End(); 
            base.Draw(gameTime);
        }
    }
}
