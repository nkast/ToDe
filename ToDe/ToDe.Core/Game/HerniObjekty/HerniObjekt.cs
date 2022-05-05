using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal abstract class HerniObjekt
    {
        public Vector2 Pozice { get; set; }
        //public float UhelPohybu { get; set; } = 0;
        public float UhelOtoceni { get; set; } = 0;
        public float UhelKorkceObrazku { get; set; } = 0;
        public float RychlostPohybu { get; set; } = 0;
        public float RychlostRotace { get; set; } = 0;
        public float Meritko { get; set; } = 1;
        public Vector2 Stred { get; set; }
        public bool Smazat { get; set; } = false;
        public float Nepruhlednost { get; set; } = 1;

        public Color barva = Color.White;
        public Color Barva { 
            get => ObjektJeVybran ? BarvaPriVyberu : barva; 
            set => barva = value; 
        }
        
        public Color BarvaPriVyberu { get; set; } = Color.LightSalmon;
        public bool ObjektJeVybran { get; set; } = false;

        public DlazdiceUrceni[] Dlazdice { get; set; }

        public HerniObjekt()
        {
            Stred = new Vector2(Zdroje.VelikostDlazdice / 2f);
        }

        public virtual void Update(float sekundOdMinule)
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (Smazat || Nepruhlednost <= 0 || Dlazdice == null) return;
            foreach (var obr in Dlazdice)
            {
                sb.Kresli(Pozice, obr, Stred, (UhelOtoceni * (obr.Otacet ? 1 : 0)) + UhelKorkceObrazku, Meritko,
                    barva: Nepruhlednost < 1 ? Kresleni.Pruhlednost(Nepruhlednost, Barva) : Barva);
            }
        }

        public virtual void TranspozicePozice()
        {
            Pozice = new Vector2(Pozice.Y, Pozice.X);
            UhelOtoceni = TDUtils.KorekceUhlu(90 - UhelOtoceni);
        }
    }

    internal class VybranaDlazdice : HerniObjekt
    {
        public Point PoziceNaMape { get; set; }

        public VybranaDlazdice(Point poziceNaMape)
        {
            PoziceNaMape = poziceNaMape;
        }

        public override void TranspozicePozice()
        {
            base.TranspozicePozice();
            PoziceNaMape = new Point(PoziceNaMape.Y, PoziceNaMape.X);
        }
    }
}
