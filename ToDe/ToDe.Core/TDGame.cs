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
        List<Dira> diry;
        List<PolozkaNabidky> nabidka;
        protected override void Initialize()
        {
            nepratele = new List<Nepritel>();
            diry = new List<Dira>();
            nabidka = new List<PolozkaNabidky>();

            base.Initialize();
        }

        Mapa aktualniMapa;
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Mapa.Textura = Content.Load<Texture2D>(@"Sprites/Basic");
            Mapa.Pismo = Content.Load<SpriteFont>(@"Fonts/Pismo");

            aktualniMapa = Mapa.NactiMapu(1);

            nabidka.Add(new PolozkaNabidky(0, TypPolozkyNabidky.Vez1));
            nabidka.Add(new PolozkaNabidky(1, TypPolozkyNabidky.Text) { Text = "10" });
            nabidka.Add(new PolozkaNabidky(3, TypPolozkyNabidky.Vez2));
            nabidka.Add(new PolozkaNabidky(4, TypPolozkyNabidky.Text) { Text = "5" });
            nabidka.Add(new PolozkaNabidky(-1, TypPolozkyNabidky.Text) { Text = "100%" });

            nepratele.Add(new Nepritel());
            diry.Add(new Dira()
            {
                Pozice = new Vector2(3 * 128 + 64, 4 * 128 + 64),
                RychlostMizeni = 0.075f,
            }); ;


            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Vector2 poziceKliknuti = Vector2.Zero;
            // Dotyk
            poziceKliknuti = TouchPanel.GetState().FirstOrDefault(tl => tl.State == TouchLocationState.Pressed).Position;
            // Myš
            var ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
                poziceKliknuti = new Vector2(ms.X, ms.Y);
            poziceKliknuti /= globalniMeritko;

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            nepratele.ForEach(x => x.Update(seconds));
            diry.ForEach(x => x.Update(seconds));
            nabidka.ForEach(x => x.Update(seconds, poziceKliknuti));

            nepratele.RemoveAll(x => x.Smazat);
            diry.RemoveAll(x => x.Smazat);
            base.Update(gameTime);
        }

        float globalniMeritko = 1;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // Barva podkladu na pozadí

            // Matice pro měřítko (zoom) vykreslování, aby se vše vešlo do okna
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)(aktualniMapa.Sloupcu* Mapa.VelikostDlazdice);
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)((aktualniMapa.Radku + 1) * Mapa.VelikostDlazdice);
            globalniMeritko = MathHelper.Min(scaleX, scaleY);
            var screenScale = new Vector3(new Vector2(globalniMeritko), 1.0f);
            var scaleMatrix = Matrix.CreateScale(screenScale);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, transformMatrix: scaleMatrix); // Začátek vykreslování


            // Vykreslení pozadí mapy
            for (int i = 0; i < aktualniMapa.Radku; i++)
                for (int j = 0; j < aktualniMapa.Sloupcu; j++)
                {
                    spriteBatch.Kresli(aktualniMapa.CilDlazdice(i, j), aktualniMapa.MezeDlazdice(i, j), Vector2.Zero);
                }

            // Vykreslování seznamů
            nepratele.ForEach(x => x.Draw(spriteBatch));
            diry.ForEach(x => x.Draw(spriteBatch));
            nabidka.ForEach(x => x.Draw(spriteBatch));
            nabidka.FirstOrDefault().DrawVyber(spriteBatch); // Vykreslení označovacího rámečku v nabídce


            spriteBatch.End(); // Konec vykreslování
            base.Draw(gameTime);
        }
    }
}
