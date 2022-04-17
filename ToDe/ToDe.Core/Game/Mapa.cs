using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ToDe
{
    internal enum TypDlazdice
    {
        Ground,
        Road,
        //GR_DR,
        //GR_D,
        //GR_DL,
        //GR_R,
        //GR_L,
        //GR_TR,
        //GR_T,
        //GR_TL,
    }

    public enum KamJit
    {
        Nahoru = 270,
        Doprava = 0,
        Dolu = 90,
        Doleva = 180,
    }

    internal class Mapa
    {
        public static Mapa Aktualni { get; private set; }

        public static Texture2D Textura { get; internal set; }

        public const int VelikostDlazdice = 128;


        public Dictionary<TypDlazdice, DlazdiceUrceni> TypNaDlazici { get; private set; } 
            = new Dictionary<TypDlazdice, DlazdiceUrceni>() {
                { TypDlazdice.Ground, new DlazdiceUrceni(19, 6) },
                { TypDlazdice.Road, new DlazdiceUrceni(21, 6) },
            };

        public TypDlazdice[,] Pozadi { get; private set; }
        public int Radku { get => Pozadi.GetLength(0); }
        public int Sloupcu { get => Pozadi.GetLength(1); }

        public Point Start { get; private set; }
        public Point Cil { get; private set; }
        //public Point StartPred { get; private set; }
        //public Point CilZa { get; private set; }
        public List<Point> TrasaPochodu { get; private set; }
        public Vector2 PoziceNaTrase(int indexCilovy)
        {
            if (indexCilovy <= 0 || indexCilovy >= TrasaPochodu.Count-1)
                return new Vector2(TrasaPochodu[indexCilovy].X * VelikostDlazdice + VelikostDlazdice * 0.5f,
                                   TrasaPochodu[indexCilovy].Y * VelikostDlazdice + VelikostDlazdice * 0.5f);
            var kam = SmerDalsiTrasy(TrasaPochodu[indexCilovy-1], TrasaPochodu[indexCilovy]);
            switch (kam)
            {
                case KamJit.Nahoru: 
                    return new Vector2((TrasaPochodu[indexCilovy].X + 0.5f) * VelikostDlazdice,
                                       (TrasaPochodu[indexCilovy].Y + 1) * VelikostDlazdice);
                case KamJit.Doprava:
                    return new Vector2((TrasaPochodu[indexCilovy].X) * VelikostDlazdice,
                                       (TrasaPochodu[indexCilovy].Y + 0.5f) * VelikostDlazdice);
                case KamJit.Dolu:
                    return new Vector2((TrasaPochodu[indexCilovy].X + 0.5f) * VelikostDlazdice,
                                       TrasaPochodu[indexCilovy].Y * VelikostDlazdice);
                case KamJit.Doleva:
                    return new Vector2((TrasaPochodu[indexCilovy].X + 1) * VelikostDlazdice,
                                       (TrasaPochodu[indexCilovy].Y + 0.5f) * VelikostDlazdice);
                default: return Vector2.Zero;
            }
        }
        public KamJit SmerDalsiTrasy(Point odkud, Point kam)
        {
            if (odkud.Y == kam.Y) // Půjde doleva nebo doprava
                if (odkud.X < kam.X) // Jde doprava?
                    return KamJit.Doprava;
                else
                    return KamJit.Doleva;
            else // Půjde nahoru nebo dolů
                if (odkud.Y < kam.Y) // Jde dolů?
                return KamJit.Dolu;
            else
                return KamJit.Nahoru;
        }


        int cisloMapy = 0;

        private Mapa() { }

        public static Mapa NactiMapu(int cisloMapy)
        {
            // Načtení streamu
            string soubor = string.Format("Content/Levels/Level{0}.txt", cisloMapy);
            var mapa = new Mapa();
            mapa.cisloMapy = cisloMapy;
            Aktualni = mapa;
            var radkyMapy = new List<string>();
            int? sloupcu = null;
            using (Stream fileStream = TitleContainer.OpenStream(soubor))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (sloupcu == null)
                        sloupcu = line.Length;
                    else
                        if (line.Length != sloupcu)
                            throw new Exception($"Mapa Level{cisloMapy} nemá na všech řádcích stejný počet znaků");
                    radkyMapy.Add(line);
                }
            }
            // Vytvoření mapy z načtených dat
            mapa.Pozadi = new TypDlazdice[radkyMapy.Count, sloupcu.Value];
            for (int i = 0; i < radkyMapy.Count; i++)
            {
                for (int j = 0; j < sloupcu.Value; j++)
                {
                    mapa.Pozadi[i, j] = ZnakNaTyp(radkyMapy[i][j], i, j);
                }
            }
            // Nalezení parametrů pro cestu
            mapa.NajdiCestu();
            return mapa;
        }

        static TypDlazdice ZnakNaTyp(char tile, int i, int j)
        {
            switch (tile)
            {
                case '.': return TypDlazdice.Ground;
                case 'S': Aktualni.Start = new Point(j, i); return TypDlazdice.Road;
                case 'E': Aktualni.Cil = new Point(j, i); return TypDlazdice.Road;
                case '*': return TypDlazdice.Road;
                default: return TypDlazdice.Ground;
            }
        }


        void NajdiCestu()
        {
            // Nalezení cesty ze startu do cíle
            TrasaPochodu = new List<Point>();
            var navstivenePozice = new List<Point>();
            if (!NajdiCestuProhledejPole(Start, navstivenePozice))
                throw new Exception($"Mapa Level{cisloMapy} nemá cestu ze startu do cíle");
            TrasaPochodu.Reverse();
            // Přidání startu za okraj mapy
            if (Start.X == 0) TrasaPochodu.Insert(0, new Point(-1, Start.Y));
            else if (Start.X == Sloupcu-1) TrasaPochodu.Insert(0, new Point(Sloupcu, Start.Y));
            else if (Start.Y == 0) TrasaPochodu.Insert(0, new Point(Start.X, -1));
            else if (Start.Y == Radku-1) TrasaPochodu.Insert(0, new Point(Start.X, Radku));
            // Přidání cíle za okraj mapy
            if (Cil.X == Sloupcu-1) TrasaPochodu.Add(new Point(Sloupcu, Start.Y));
            else if (Cil.X == 0) TrasaPochodu.Add(new Point(-1, Start.Y));
            else if (Cil.Y == 0) TrasaPochodu.Add(new Point(Start.X, -1));
            else if (Cil.Y == Radku-1) TrasaPochodu.Add(new Point(Start.X, Radku));
        }
        bool NajdiCestuProhledejPole(Point souradnice, List<Point> navstivenePozice)
        {
            if (souradnice.X < 0 || souradnice.Y < 0 || souradnice.X >= Sloupcu || souradnice.Y >= Radku ||
                navstivenePozice.Contains(souradnice) || Pozadi[souradnice.Y, souradnice.X] != TypDlazdice.Road)
                return false;

            navstivenePozice.Add(souradnice);
            if (souradnice == Cil)
            {
                TrasaPochodu.Add(souradnice);
                return true;
            }

            foreach (var p in new[] { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) })
                if (NajdiCestuProhledejPole(souradnice + p, navstivenePozice))
                {
                    TrasaPochodu.Add(souradnice);
                    return true;
                }
            return false;
        }


        public DlazdiceUrceni MezeDlazdice(int i, int j)
            => TypNaDlazici[Pozadi[i, j]];

        public Rectangle CilDlazdice(int i, int j)
            => new Rectangle(j * VelikostDlazdice, i * VelikostDlazdice, VelikostDlazdice, VelikostDlazdice);
    }
}
