using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal class Kruh : HerniObjekt
    {
        Vez vez;

        public Kruh(Vez vez)
        {
            this.vez = vez;
            Stred = new Vector2(Zdroje.Obsah.Kruh.VelikostDlazdice.X / 2f);
            Pozice = vez.Pozice;
        }

        public override void Update(float sekundOdMinule)
        {
            base.Update(sekundOdMinule);

            if (vez == null || vez.Smazat || !vez.ObjektJeVybran)
            {
                Smazat = true;
                return;
            }

            Pozice = vez.Pozice;
            Meritko = 2 * vez.DosahStrelby / Zdroje.Obsah.Kruh.VelikostDlazdice.X;
        }

        public override void Draw(SpriteBatch sb)
        {
            // base.Draw(sb);
            if (!Smazat)
                sb.Draw(Zdroje.Obsah.Kruh.Grafika, Pozice, null, Barva, UhelOtoceni, Stred, Meritko, SpriteEffects.None, 0.9999f);
        }

    }
}
