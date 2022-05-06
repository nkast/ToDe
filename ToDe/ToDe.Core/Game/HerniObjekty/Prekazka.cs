using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal class Prekazka : HerniObjekt
    {
        public Point SouradniceNaMape { get; private set; }

        public char ZnakPrekazky { get; private set; }

        public Prekazka(char znakPrekazky, Point souradniceNaMape)
        {
            ZnakPrekazky = znakPrekazky;
            SouradniceNaMape = souradniceNaMape;
            UhelKorkceObrazku = TDUtils.RND.Next(4) * 90;
            Dlazdice = new[] {
                new DlazdiceUrceni(KonfiguracePrekazek.DlazdicePrekazky(znakPrekazky), 0.1f, false),
            };
            Pozice = new Vector2((souradniceNaMape.X + 0.5f) * Zdroje.VelikostDlazdice,
                                 (souradniceNaMape.Y + 0.5f) * Zdroje.VelikostDlazdice);
        }

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
            SouradniceNaMape = new Point(SouradniceNaMape.Y, SouradniceNaMape.X);
        }
    }
}
