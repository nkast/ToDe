using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

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

    internal struct MezeryOdOkraju 
    {
        public float Vlevo;
        public float Nahore;
        public float Vpravo;
        public float Dole;

        public float Horizontalne { 
            get => Vlevo + Vpravo;
            set => (Vlevo, Vpravo) = (value, value);
        }

        public float Vertikalne
        {
            get => Nahore + Dole;
            set => (Nahore, Dole) = (value, value);
        }

        public MezeryOdOkraju(float vlevo, float nahore, float vpravo, float dole)
            => (Vlevo, Nahore, Vpravo, Dole) = (vlevo, nahore, vpravo, dole);
        public MezeryOdOkraju(float horizontalne, float vertikalne, float mezi = 0)
            => (Vlevo, Nahore, Vpravo, Dole) = (horizontalne, vertikalne, horizontalne, vertikalne);
        public MezeryOdOkraju(float vse, float mezi = 0)
            => (Vlevo, Nahore, Vpravo, Dole) = (vse, vse, vse, vse);

        public static implicit operator MezeryOdOkraju(float vse) => new MezeryOdOkraju(vse);
        public MezeryOdOkraju(MezeryOdOkraju vzor, float? vlevo = null, float? nahore = null, float? vpravo = null, float? dole = null)
            => (Vlevo, Nahore, Vpravo, Dole) = (vlevo ?? vzor.Vlevo, nahore ?? vzor.Nahore, vpravo ?? vzor.Vpravo, dole ?? vzor.Dole);

        public override string ToString() => $"{Vlevo}, {Nahore}, {Vpravo}, {Dole}";
    }


    internal static class Rozsireni
    {
        //public static Rectangle Plus(this Rectangle rec, int plusX, int plusY, int plusSirka, int plusVyska)
        //    => new Rectangle(rec.Left + plusX, rec.Top + plusY, rec.Width + plusSirka, rec.Height + plusVyska);

        public static T GetAtt<T>(this XElement element, XName attributeName, T defaultValue)
            => (T)AttributeValue(typeof(T), element, attributeName, defaultValue);

        public static T AttVal<T>(XElement element, XName attributeName, T defaultValue)
            => (T)AttributeValue(typeof(T), element, attributeName, defaultValue);


        public static object AttributeValue(Type type, XElement element, XName attributeName, object defaultValue)
        {
            XAttribute attribute = null;
            if (element != null)
                attribute = element.Attribute(attributeName);
            if (attribute == null || String.IsNullOrEmpty(attribute.Value))
                return defaultValue;
            if (type == typeof(DateTime))
                return (DateTime)attribute;
            return ConvertXmlValue(type, attribute.Value, defaultValue);
        }

        public static object ConvertXmlValue(Type type, string value, object defaultValue)
        {
            try
            {
                if (String.IsNullOrEmpty(value))
                    return defaultValue;
                if (type == typeof(double))
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                if (type == typeof(float))
                    return Convert.ToSingle(value, CultureInfo.InvariantCulture);
                if (type == typeof(decimal))
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                if (type == typeof(ushort))
                    return Convert.ToUInt16(value);
                if (type == typeof(byte))
                    return Convert.ToByte(value);
                if (type == typeof(short))
                    return Convert.ToInt16(value);
                if (type == typeof(int))
                    return Convert.ToInt32(value);
                if (type == typeof(int?))
                    return (int?)Convert.ToInt32(value);
                if (type == typeof(long))
                    return Convert.ToInt64(value);
                if (type == typeof(long?))
                    return (long?)Convert.ToInt64(value);
                if (type == typeof(DateTime))
                    return DateTime.Parse(value);
                //if (type == typeof(Color))
                //    return Color.FromHex(value.TrimStart('#'));
                if (type == typeof(bool) || type == typeof(bool?))
                {
                    bool result = (!String.IsNullOrEmpty(value) &&
                        (value == "1" || value.ToLower() == "true"));
                    if (type == typeof(bool?))
                        return (bool?)result;
                    return result;
                }
                if (type.GetTypeInfo().IsSubclassOf(typeof(Enum)))
                    return Enum.Parse(type, value, true);
                return value;
            }
            catch { }
            return defaultValue;
        }

    }

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

        public static void NactiParametrVeze(LevelVez vez, XElement eVez, XElement eUroven, ushort idUrovne, string nazevVlastnosti, int nasobitel = 1) 
        {
            Type typ = null;
            switch (vez.Typ)
            {
                case TypVeze.Kulomet: typ = typeof(KonfiguraceVezKulomet); break;
                case TypVeze.Raketa:  typ = typeof(KonfiguraceVezRaketa); break;
            }
            var prop = typ.GetProperty(nazevVlastnosti);
            string nazevAtributu = nazevVlastnosti[0].ToString().ToLower() + nazevVlastnosti.Substring(1); // Název atributu = název vlasntosti, ale první písmeno je malé
            var metoda = typ.GetMethod(nameof(KonfiguraceVezKulomet.ParametryVeze));
            var vlastnostiSVychoziHodnotou = metoda.Invoke(null, new object[] { idUrovne }) as KonfiguraceVeze; // KonfiguraceVezKulomet.ParametryVeze(idUrovne)

            //var eUroven = eVez.Elements().FirstOrDefault(x => Convert.ToInt16(x.Attribute("id").Value) == idUrovne);
            float hodnota;
            if (idUrovne == 1)
                hodnota = eUroven.GetAtt(nazevAtributu,
                    (float)prop.GetValue(vlastnostiSVychoziHodnotou)) * nasobitel;
            else
            {
                string val = eUroven.GetAtt(nazevAtributu, "");
                if (String.IsNullOrEmpty(val))
                    hodnota = (float)prop.GetValue(vlastnostiSVychoziHodnotou) * nasobitel;
                else
                {
                    if (val[0] == '+' || val[0] == '-')
                        hodnota = (float)prop.GetValue(vez.Vlasntosti[(ushort)(idUrovne - 1)])
                            * (1 + Convert.ToSingle(val));
                    else
                        hodnota = Convert.ToSingle(val) * nasobitel;
                }
            }

            prop.SetValue(vez.Vlasntosti[idUrovne], hodnota);

        }
    }

}
