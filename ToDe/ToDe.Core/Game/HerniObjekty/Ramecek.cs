using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal class Ramecek : HerniObjekt
    {
        public Point PoziceNaMape { get; set; }
        public bool Viditelny { get; set; } = false;

        public Ramecek()
        {
            PoziceNaMape = Point.Zero;
            Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Nabidka_Vyber, 0.99f, false) };
            Pozice = new Vector2((PoziceNaMape.X + 0.5f) * Zdroje.VelikostDlazdice, (PoziceNaMape.Y + 0.5f) * Zdroje.VelikostDlazdice);
        }

        public void Update(float sekundOdMinule, Vector2 klik)
        {
            Update(sekundOdMinule);

            // Pozice netextových položek nabídky - střed dlaždice
            Pozice = new Vector2((PoziceNaMape.X + 0.5f) * Zdroje.VelikostDlazdice, (PoziceNaMape.Y + 0.5f) * Zdroje.VelikostDlazdice);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Viditelny)
                base.Draw(sb);
        }


        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
            PoziceNaMape = new Point(PoziceNaMape.Y, PoziceNaMape.X);
        }
    }
}
