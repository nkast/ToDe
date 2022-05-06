using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToDe
{
    internal static class Kresleni
    {
        public static void Kresli(this SpriteBatch sb,
            Vector2 pozice,
            DlazdiceUrceni dlazdice,
            Vector2? stred = null,
            float uhelOtoceni = 0,
            float meritko = 1,
            SpriteEffects efekt = SpriteEffects.None,
            Color? barva = null,
            Point? velikostDlazdice = null,
            Textura textura = null
            )
        {
            if (!dlazdice.Vykreslovat) return;

            Point vd = velikostDlazdice ?? textura?.VelikostDlazdice ?? new Point(Zdroje.VelikostDlazdice);

            sb.Draw(textura?.Grafika ?? Zdroje.Obsah.Zakladni.Grafika,
                  position: pozice,
                  sourceRectangle: textura == null ? dlazdice.VyrezZeZakladniTextury() : dlazdice.VyrezTextury(textura),
                  rotation: MathHelper.ToRadians(uhelOtoceni),
                  origin: stred ?? new Vector2(vd.X, vd.Y) * 0.5f,
                  scale: meritko,
                  effects: efekt,
                  color: barva ?? Color.White,
                  layerDepth: dlazdice.Z);
        }

        public static void Kresli(this SpriteBatch sb,
            Vector2 pozice,
            Rectangle vyrezZTextury,
            Vector2? stred = null,
            float uhelOtoceni = 0,
            float meritko = 1,
            float Z = 0,
            SpriteEffects efekt = SpriteEffects.None,
            Color? barva = null,
            Textura textura = null
            )
        {
            sb.Draw(textura?.Grafika ?? Zdroje.Obsah.Zakladni.Grafika,
                  position: pozice,
                  sourceRectangle: vyrezZTextury,
                  rotation: MathHelper.ToRadians(uhelOtoceni),
                  origin: stred ?? new Vector2(vyrezZTextury.Width * 0.5f, vyrezZTextury.Height * 0.5f),
                  scale: meritko,
                  effects: efekt,
                  color: barva ?? Color.White,
                  layerDepth: Z);
        }

        public static void Kresli(this SpriteBatch sb,
            Rectangle cil,
            DlazdiceUrceni dlazdice,
            Vector2? stred = null,
            float uhelOtoceni = 0,
            SpriteEffects efekt = SpriteEffects.None,
            Color? barva = null,
            Point? velikostDlazdice = null,
            Textura textura = null
            )
        {
            if (!dlazdice.Vykreslovat) return;
           
            Point vd = velikostDlazdice ?? textura?.VelikostDlazdice ?? new Point(Zdroje.VelikostDlazdice);

            sb.Draw(textura?.Grafika ?? Zdroje.Obsah.Zakladni.Grafika,
                  destinationRectangle: cil,
                  sourceRectangle: textura == null ? dlazdice.VyrezZeZakladniTextury() : dlazdice.VyrezTextury(textura), 
                  rotation: MathHelper.ToRadians(uhelOtoceni),
                  origin: stred ?? new Vector2(vd.X, vd.Y) * 0.5f,
                  effects: efekt,
                  color: barva ?? Color.White,
                  layerDepth: dlazdice.Z);
        }

        public static Color Pruhlednost(float nepruhlednost, Color barva)
            => new Color(barva.R, barva.G, barva.B, (int)(255 * nepruhlednost));

        public static void KresliTextDoprostred(this SpriteBatch sb, string text)
        {
            var textSize = Zdroje.Obsah.Pismo.MeasureString(text);
            sb.DrawString(Zdroje.Obsah.Pismo, text,
                new Vector2(Zdroje.Aktualni.Level.Mapa.Sloupcu * Zdroje.VelikostDlazdice * 0.5f,
                            Zdroje.Aktualni.Level.Mapa.Radku * Zdroje.VelikostDlazdice * 0.5f),
                Color.White, 0, new Vector2(textSize.X * 0.5f, textSize.Y * 0.5f), 1, SpriteEffects.None, 1);
        }
    }

    internal struct DlazdiceUrceni
    {
        public int X; // Pozice X dlaždice na textuře
        public int Y; // Pozice Y dlaždice na textuře
        public float Z; // Vrstva pro vykreslení dlaždice
        public bool Otacet;
        public bool Vykreslovat;

        private DlazdiceUrceni(int x, int y, float z = 0, bool otacet = true) 
            => (X, Y, Z, Otacet, Vykreslovat) = (x, y, z, otacet, true);
        private DlazdiceUrceni(Point souradnice, float z = 0, bool otacet = true)
            => (X, Y, Z, Otacet, Vykreslovat) = (souradnice.X, souradnice.Y, z, otacet, true);
        public DlazdiceUrceni(ZakladniDlazdice zd, float z = 0, bool otacet = true)
        {
            var souradnice = Zdroje.Obsah.Zakladni.SouradniceDlazdice(zd);
            (X, Y, Z, Otacet, Vykreslovat) = (souradnice.X, souradnice.Y, z, otacet, true);
        }

        public Rectangle VyrezZeZakladniTextury()
            => new Rectangle(X * (Zdroje.VelikostDlazdice + 2 * Zdroje.Obsah.Zakladni.Okraj) + Zdroje.Obsah.Zakladni.Okraj,
                             Y * (Zdroje.VelikostDlazdice + 2 * Zdroje.Obsah.Zakladni.Okraj) + Zdroje.Obsah.Zakladni.Okraj,
                             Zdroje.VelikostDlazdice, Zdroje.VelikostDlazdice);

        public Rectangle VyrezTextury(Textura textura)
            => new Rectangle(X * (textura.VelikostDlazdice.X + 2 * textura.Okraj) + textura.Okraj,
                             Y * (textura.VelikostDlazdice.Y + 2 * textura.Okraj) + textura.Okraj,
                             textura.VelikostDlazdice.X, textura.VelikostDlazdice.Y);
    }

    internal struct PrekazkaNaMape
    {
        public Point Pozice;
        public char Znak;

        public PrekazkaNaMape(Point pozice, char znak)
            => (Pozice, Znak) = (pozice, znak);
    }

    //internal static class Rozsireni {
    //    public static Rectangle Plus(this Rectangle rec, int plusX, int plusY, int plusSirka, int plusVyska)
    //        => new Rectangle(rec.Left + plusX, rec.Top + plusY, rec.Width + plusSirka, rec.Height + plusVyska);
    //}

    internal static class TDUtils
    {
        public static Random RND = new Random();

        public static Vector2 PosunPoUhlu(float uhel, float rychlost)
        {
            var radiany = MathHelper.ToRadians(uhel);
            return new Vector2((float)Math.Cos(radiany) * rychlost,
                               (float)Math.Sin(radiany) * rychlost);
        }

        public static float OtacejSeKCili(float elapsedSeconds, 
            Vector2 poziceObjektu, Vector2 poziceCile,
            float uhelObjektu, float rychlostRotace, out bool muzeStrilet)
        {
            float ang = MathHelper.ToDegrees((float)Math.Atan2(poziceCile.Y - poziceObjektu.Y, 
                                                               poziceCile.X - poziceObjektu.X));
            float rozdilUhlu = RozdilUhlu(uhelObjektu, ang);

            if (Math.Abs(rozdilUhlu) > rychlostRotace * elapsedSeconds)
            {
                muzeStrilet = false;
                return uhelObjektu + Math.Sign(rozdilUhlu) * (rychlostRotace * elapsedSeconds);
            }
            muzeStrilet = true;
            return ang;
        }

        public static float KorekceUhlu(float angle)
        {
            if (angle > 360)
                angle = angle % 360;
            while (angle < 0)
                angle += 360; // TODO: vypočíst bez cyklu
            return angle;
        }

        static float RozdilUhlu(float aktualniUhel, float cilovyUhel)
        {
            aktualniUhel = KorekceUhlu(aktualniUhel);
            cilovyUhel = KorekceUhlu(cilovyUhel);

            float rozdil = cilovyUhel - aktualniUhel;

            if (rozdil > 180)
                return -(360 - rozdil);
            if (rozdil < -180)
                return rozdil + 360;
            return rozdil;
        }
    }

}
