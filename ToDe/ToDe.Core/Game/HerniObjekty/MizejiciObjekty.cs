using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal abstract class MizejiciObjekt : HerniObjekt
    {
        public float RychlostMizeni { get; set; } // -% za s

        public override void Update(float sekundOdMinule)
        {
            Nepruhlednost -= RychlostMizeni * sekundOdMinule;
            if (Nepruhlednost <= 0)
            {
                Nepruhlednost = 0;
                Smazat = true;
            }

            base.Update(sekundOdMinule);
        }
    }

    internal class Dira : MizejiciObjekt
    {
        public Dira()
        {
            Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Dira, 0.000001f) };
            RychlostMizeni = (float)(TDUtils.RND.NextDouble() * 0.3 + 0.1);
            UhelOtoceni = TDUtils.RND.Next(360);
        }
    }

    internal class PlamenStrelby : MizejiciObjekt
    {
        VezKulomet vez;

        public PlamenStrelby(VezKulomet vez)
        {
            this.vez = vez;
            Dlazdice = new[] { new DlazdiceUrceni(ZakladniDlazdice.Ohen_Kulomet, 0.61f) };
            RychlostMizeni = Math.Max(1 / vez.SekundMeziVystrely, 2.0f);
            UhelKorkceObrazku = 90;
            Meritko = 0.5f;
            UhelOtoceni = vez.UhelOtoceni;
            Pozice = vez.Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, Zdroje.VelikostDlazdice * 0.55f);
        }

        public override void Update(float sekundOdMinule)
        {
            UhelOtoceni = vez.UhelOtoceni;
            Pozice = vez.Pozice + TDUtils.PosunPoUhlu(UhelOtoceni, Zdroje.VelikostDlazdice * 0.55f);
            base.Update(sekundOdMinule);
        }
    }
}
