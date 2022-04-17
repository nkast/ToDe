﻿using Microsoft.Xna.Framework;
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
            int? velikostDlazdice = null,
            Texture2D textura = null
            )
        {
            int vd = velikostDlazdice ?? Mapa.VelikostDlazdice;

            sb.Draw(textura ?? Mapa.Textura,
                  position: pozice,
                  sourceRectangle: new Rectangle(dlazdice.X * vd, dlazdice.Y * vd, vd, vd),
                  rotation: MathHelper.ToRadians(uhelOtoceni),
                  origin: stred ?? new Vector2(vd * 0.5f),
                  scale: meritko,
                  effects: efekt,
                  color: barva ?? Color.White,
                  layerDepth: dlazdice.Z);
        }

        public static void Kresli(this SpriteBatch sb,
            Rectangle cil,
            DlazdiceUrceni dlazdice,
            Vector2? stred = null,
            float uhelOtoceni = 0,
            SpriteEffects efekt = SpriteEffects.None,
            Color? barva = null,
            int? velikostDlazdice = null,
            Texture2D textura = null
            )
        {
            int vd = velikostDlazdice ?? Mapa.VelikostDlazdice;

            sb.Draw(textura ?? Mapa.Textura,
                  destinationRectangle: cil,
                  sourceRectangle: new Rectangle(dlazdice.X * vd, dlazdice.Y * vd, vd, vd),
                  rotation: MathHelper.ToRadians(uhelOtoceni),
                  origin: stred ?? new Vector2(vd * 0.5f),
                  effects: efekt,
                  color: barva ?? Color.White,
                  layerDepth: dlazdice.Z);
        }

        public static Color Pruhlednost(float nepruhlednost)
            => new Color(255, 255, 255, (int)(255 * nepruhlednost));
    }

    internal struct DlazdiceUrceni
    {
        public int X; // Pozice X dlaždice na textuře
        public int Y; // Pozice Y dlaždice na textuře
        public float Z; // Vrstva pro vykreslení dlaždice

        public DlazdiceUrceni(int x, int y, float z = 0) => (X, Y, Z) = (x, y, z);
    }

    internal static class Rozsireni {
        public static Rectangle Plus(this Rectangle rec, int plusX, int plusY, int plusSirka, int plusVyska)
            => new Rectangle(rec.Left + plusX, rec.Top + plusY, rec.Width + plusSirka, rec.Height + plusVyska);
    }

}
