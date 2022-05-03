using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal class UkazatelZdravi : HerniObjekt
    {
        Nepritel nepritel;
        float sirkaUkazateleJakoProcentoZeSirkyDlazdice = 1;
        float vzdalenostUkazateleJakoProcentoZeSirkyDlazdice = 0.5f;
        ushort sirkaUkazatele, vyskaUkazatele;
        float vzdalenostUkazatele;

        public UkazatelZdravi(Nepritel nepritel)
        {
            this.nepritel = nepritel;
            Pozice = nepritel.Pozice;
            //Meritko = 0.5f;

            switch (nepritel.Typ) // TODO: udělat na to slovník poblíž tohoto výčtu
            {
                case TypNepritele.Parasutista:
                case TypNepritele.Robot:
                case TypNepritele.Vojak:
                case TypNepritele.Ufon:
                    sirkaUkazateleJakoProcentoZeSirkyDlazdice = 0.5f;
                    vzdalenostUkazateleJakoProcentoZeSirkyDlazdice = 0.4f;
                    break;
                case TypNepritele.Tank:
                case TypNepritele.TankPoustni:
                    sirkaUkazateleJakoProcentoZeSirkyDlazdice = 0.9f;
                    vzdalenostUkazateleJakoProcentoZeSirkyDlazdice = 0.5f;
                    break;
                default:
                    sirkaUkazateleJakoProcentoZeSirkyDlazdice = 1;
                    vzdalenostUkazateleJakoProcentoZeSirkyDlazdice = 0.5f;
                    break;
            }
            sirkaUkazatele = (ushort)(Zdroje.VelikostDlazdice * sirkaUkazateleJakoProcentoZeSirkyDlazdice);
            vyskaUkazatele = (ushort)(Zdroje.Obsah.Ukazatel.VelikostDlazdice.Y * 0.5f);
            vzdalenostUkazatele = vzdalenostUkazateleJakoProcentoZeSirkyDlazdice * Zdroje.VelikostDlazdice;
            Stred = new Vector2(0, vyskaUkazatele);
            Pozice = nepritel.Pozice + new Vector2(-sirkaUkazatele * 0.5f, -vzdalenostUkazatele);
        }

        public override void Update(float sekundOdMinule)
        {
            base.Update(sekundOdMinule);
            Pozice = nepritel.Pozice + new Vector2(-sirkaUkazatele * 0.5f, -vzdalenostUkazatele);
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(sb);
            int sirkaZelene = (int)(sirkaUkazatele * nepritel.ProcentoZdravi);
            // Zelená
            if (nepritel.ProcentoZdravi > 0)
                sb.Kresli(Pozice,
                    new Rectangle(Zdroje.Obsah.Ukazatel.Okraj, Zdroje.Obsah.Ukazatel.Okraj,
                        sirkaZelene, vyskaUkazatele), 
                    Vector2.Zero, 
                    textura: Zdroje.Obsah.Ukazatel, Z: 0.911f);
            // Červená
            if (nepritel.ProcentoZdravi < 1)
                sb.Kresli(Pozice + new Vector2(sirkaZelene - 1, 0),
                    new Rectangle(Zdroje.Obsah.Ukazatel.Okraj, 
                        Zdroje.Obsah.Ukazatel.VelikostDlazdice.Y + 3 * Zdroje.Obsah.Ukazatel.Okraj,
                        sirkaUkazatele - sirkaZelene + 1, vyskaUkazatele),
                    Vector2.Zero, textura: Zdroje.Obsah.Ukazatel, Z: 0.91f);
        }
    }
}
