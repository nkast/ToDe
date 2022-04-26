using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
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
        public Exploze(Raketa raketa) : base(Zdroje.Obsah.Exploze.Grafika, 8, 6)
        {
            Pozice = raketa.Pozice;
            UhelOtoceni = TDUtils.RND.Next(360);
            Meritko = 0.75f;
            Z = 0.5f;
            RychlostAnimace = 25f;
        }
    }

}
