using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal class Prekazka : HerniObjekt
    {
        public Point SouradniceNaMape { get; set; }

        char znakPrekazky;

        public Prekazka(char znakPrekazky, Point souradniceNaMape)
        {
            this.znakPrekazky = znakPrekazky;
            SouradniceNaMape = souradniceNaMape;
            UhelKorkceObrazku = TDUtils.RND.Next(4) * 90;
            Dlazdice = new[] {
                new DlazdiceUrceni(KonfiguracePrekazek.DlazdicePrekazky(znakPrekazky), 0.1f, false),
            };
        }

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
            SouradniceNaMape = new Point(SouradniceNaMape.Y, SouradniceNaMape.X);
        }
    }
}
