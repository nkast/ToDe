using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        protected override void Initialize()
        {
            nepratele = new List<Nepritel>();
            diry = new List<Dira>();

            base.Initialize();
        }

        Mapa aktualniMapa;
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Mapa.Textura = Content.Load<Texture2D>(@"Sprites/Basic");

            aktualniMapa = Mapa.NactiMapu(1);

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
            nepratele.ForEach(x => x.Update((float)gameTime.ElapsedGameTime.TotalSeconds));
            diry.ForEach(x => x.Update((float)gameTime.ElapsedGameTime.TotalSeconds));

            nepratele.RemoveAll(x => x.Smazat);
            diry.RemoveAll(x => x.Smazat);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // Barva podkladu na pozadí

            // Matice pro měřítko (zoom) vykreslování, aby se vše vešlo do okna
            var scaleX = (float)GraphicsDevice.Viewport.Width / (float)(aktualniMapa.Sloupcu* Mapa.VelikostDlazdice);
            var scaleY = (float)GraphicsDevice.Viewport.Height / (float)(aktualniMapa.Radku * Mapa.VelikostDlazdice);
            var screenScale = new Vector3(new Vector2(MathHelper.Min(scaleX, scaleY)), 1.0f);
            var scaleMatrix = Matrix.CreateScale(screenScale);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, transformMatrix: scaleMatrix); // Začátek vykreslování


            // Vykreslení pozadí mapy
            for (int i = 0; i < aktualniMapa.Radku; i++)
                for (int j = 0; j < aktualniMapa.Sloupcu; j++)
                {
                    spriteBatch.Kresli(aktualniMapa.CilDlazdice(i, j), aktualniMapa.MezeDlazdice(i, j), Vector2.Zero);
                }

            // Vykreslování nepřátel
            nepratele.ForEach(x => x.Draw(spriteBatch));
            diry.ForEach(x => x.Draw(spriteBatch));


            spriteBatch.End(); // Konec vykreslování
            base.Draw(gameTime);
        }
    }
}
