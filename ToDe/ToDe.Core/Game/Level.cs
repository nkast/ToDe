using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ToDe
{
    internal class LevelVlnaJednotka
    {
        public TypNepritele Typ { get; set; }
        public int Pocet { get; set; }
        public float Rozestup { get; set; }
        public float Zdravi { get; set; }
        public float Sila { get; set; } // Kolik ubere zdravi hráči, když dojde na konec
        public float Rychlost { get; set; } // Dlaždice za sekundu
        public float Odstup { get; set; } // Počet sekund, než se začne vypouštět tato jednotka

        public DlazdiceUrceni[] Dlazdice() => DlazdiceZTypu(Typ);

        public static DlazdiceUrceni[] DlazdiceZTypu(TypNepritele typ)
        {
            if (typ == TypNepritele.Parasutista)
                return new[] { new DlazdiceUrceni(ZakladniDlazdice.Vojak_Parasutista, 0.101f) };
            if (typ == TypNepritele.Robot)
                return new[] { new DlazdiceUrceni(ZakladniDlazdice.Vojak_Robot, 0.101f) };
            if (typ == TypNepritele.Vojak)
                return new[] { new DlazdiceUrceni(ZakladniDlazdice.Vojak_Vojak, 0.1f) };
            if (typ == TypNepritele.Ufon)
                return new[] { new DlazdiceUrceni(ZakladniDlazdice.Vojak_Ufon, 0.1f) };
            if (typ == TypNepritele.Tank)
                return new[] { 
                    new DlazdiceUrceni(ZakladniDlazdice.Tank_Spodek, 0.11f),
                    new DlazdiceUrceni(ZakladniDlazdice.Tank_Vrsek, 0.111f),
                };
            if (typ == TypNepritele.TankPoustni)
                return new[] {
                    new DlazdiceUrceni(ZakladniDlazdice.Tank_Poustni_Spodek, 0.11f),
                    new DlazdiceUrceni(ZakladniDlazdice.Tank_Poustni_Vrsek, 0.111f),
                };
            return null;
        }
    }

    internal class LevelVlna
    {
        public LevelVlnaJednotka[] Jednotky { get; set; }
        public float OdstupZacatkuVlny { get; set; }
    }

    internal class LevelVez
    {
        public TypVeze Typ { get; set; }
        public int Pocet { get; set; }
        public short MaxUroven { get; set; }
    }

    public enum TypNepritele
    {
        Parasutista,
        Robot,
        Vojak,
        Ufon,
        Tank,
        TankPoustni,
    }

    public enum TypVeze
    {
        Kulomet,
        Raketa,
    }

    internal class LevelMapa
    {
        public TypDlazdice[,] Pozadi { get; private set; }
        public int Radku { get => Pozadi.GetLength(0); }
        public int Sloupcu { get => Pozadi.GetLength(1); }

        public GrafikaDlazdice TexturaPlochy { get; set; }
        public GrafikaDlazdice TexturaCesty { get; set; }

        public Point Start { get; private set; }
        public Point Cil { get; private set; }

        public int VelikostDlazdice => Zdroje.VelikostDlazdice;
        public int OkrajDlazdice => Zdroje.Obsah.Zakladni.Okraj;

        public List<Point> TrasaPochodu { get; private set; }
        public Vector2 PoziceNaTrase(int indexCilovy)
        {
            if (indexCilovy <= 0 || indexCilovy >= TrasaPochodu.Count - 1)
                return new Vector2(TrasaPochodu[indexCilovy].X * VelikostDlazdice + VelikostDlazdice * 0.5f,
                                   TrasaPochodu[indexCilovy].Y * VelikostDlazdice + VelikostDlazdice * 0.5f);
            var kam = SmerDalsiTrasy(TrasaPochodu[indexCilovy - 1], TrasaPochodu[indexCilovy]);
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

        public static LevelMapa NactiMapu(XElement eMapa)
        {
            var mapa = new LevelMapa();
            mapa.TexturaPlochy = (GrafikaDlazdice)Enum.Parse(typeof(GrafikaDlazdice),
                eMapa.Attribute("texturaPlochy") == null ? nameof(GrafikaDlazdice.Trava) : eMapa.Attribute("texturaPlochy").Value);
            mapa.TexturaCesty = (GrafikaDlazdice)Enum.Parse(typeof(GrafikaDlazdice),
                eMapa.Attribute("texturaCesty") == null ? nameof(GrafikaDlazdice.Kameni) : eMapa.Attribute("texturaCesty").Value);

            var radkyMapy = new List<string>();
            int? sloupcu = null;
            // Načtení mapy
            string mapaText = eMapa.Value.Trim();
            var radky = mapaText.Split('\n');
            foreach (string radek in radky)
            {
                string line = radek.Trim();
                if (sloupcu == null)
                    sloupcu = line.Length;
                else
                    if (line.Length != sloupcu)
                    throw new Exception($"Mapa nemá na všech řádcích stejný počet znaků");
                radkyMapy.Add(line);
            }

            // Vytvoření mapy z načtených dat
            mapa.Pozadi = new TypDlazdice[radkyMapy.Count, sloupcu.Value];
            for (int i = 0; i < radkyMapy.Count; i++)
            {
                for (int j = 0; j < sloupcu.Value; j++)
                {
                    mapa.Pozadi[i, j] = ZnakNaTyp(mapa, radkyMapy[i][j], i, j);
                }
            }

            // Nalezení parametrů pro cestu
            mapa.NajdiCestu();

            return mapa;
        }

        static TypDlazdice ZnakNaTyp(LevelMapa mapa, char tile, int i, int j)
        {
            switch (tile)
            {
                case '.': return TypDlazdice.Plocha;
                case 'S': mapa.Start = new Point(j, i); return TypDlazdice.Cesta;
                case 'E': mapa.Cil = new Point(j, i); return TypDlazdice.Cesta;
                case '*': return TypDlazdice.Cesta;
                default: return TypDlazdice.Plocha;
            }
        }


        void NajdiCestu()
        {
            // Nalezení cesty ze startu do cíle
            TrasaPochodu = new List<Point>();
            var navstivenePozice = new List<Point>();
            if (!NajdiCestuProhledejPole(Start, navstivenePozice))
                throw new Exception($"Mapa nemá cestu ze startu do cíle");
            TrasaPochodu.Reverse();
            // Přidání startu za okraj mapy
            if (Start.X == 0) TrasaPochodu.Insert(0, new Point(-1, Start.Y));
            else if (Start.X == Sloupcu - 1) TrasaPochodu.Insert(0, new Point(Sloupcu, Start.Y));
            else if (Start.Y == 0) TrasaPochodu.Insert(0, new Point(Start.X, -1));
            else if (Start.Y == Radku - 1) TrasaPochodu.Insert(0, new Point(Start.X, Radku));
            // Přidání cíle za okraj mapy
            if (Cil.X == Sloupcu - 1) TrasaPochodu.Add(new Point(Sloupcu, Cil.Y));
            else if (Cil.X == 0) TrasaPochodu.Add(new Point(-1, Cil.Y));
            else if (Cil.Y == 0) TrasaPochodu.Add(new Point(Cil.X, -1));
            else if (Cil.Y == Radku - 1) TrasaPochodu.Add(new Point(Cil.X, Radku));
        }
        bool NajdiCestuProhledejPole(Point souradnice, List<Point> navstivenePozice)
        {
            if (souradnice.X < 0 || souradnice.Y < 0 || souradnice.X >= Sloupcu || souradnice.Y >= Radku ||
                navstivenePozice.Contains(souradnice) || Pozadi[souradnice.Y, souradnice.X] != TypDlazdice.Cesta)
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


        static Dictionary<GrafikaDlazdice, DlazdiceUrceni> grafikaNaDlazdici =
            new Dictionary<GrafikaDlazdice, DlazdiceUrceni>() {
                { GrafikaDlazdice.Trava, new DlazdiceUrceni(ZakladniDlazdice.Mapa_Trava)  },
                { GrafikaDlazdice.Hlina, new DlazdiceUrceni(ZakladniDlazdice.Mapa_Hlina)  },
                { GrafikaDlazdice.Kameni, new DlazdiceUrceni(ZakladniDlazdice.Mapa_Kameni)  },
                { GrafikaDlazdice.Pisek, new DlazdiceUrceni(ZakladniDlazdice.Mapa_Pisek)  },
            };

        Dictionary<TypDlazdice, DlazdiceUrceni> typNaDlazici;
        public Dictionary<TypDlazdice, DlazdiceUrceni> TypNaDlazici { get => typNaDlazici ??
            (typNaDlazici = new Dictionary<TypDlazdice, DlazdiceUrceni>() {
                        { TypDlazdice.Plocha, grafikaNaDlazdici[TexturaPlochy] },
                        { TypDlazdice.Cesta, grafikaNaDlazdici[TexturaCesty] },
            });
        }

        public DlazdiceUrceni MezeDlazdice(int i, int j)
            => TypNaDlazici[Pozadi[i, j]];

        public Rectangle CilDlazdice(int i, int j)
            => new Rectangle(j* VelikostDlazdice, i* VelikostDlazdice, VelikostDlazdice, VelikostDlazdice);           
    }

    internal class PolozkaPlanuVln
    {
        public float Cas { get; set; } // Čas od začátku hry v sekundách, kdy se má jednotka spustit
        public LevelVlnaJednotka Jednotka { get; set; }

        public static List<PolozkaPlanuVln> NactiPlanVln(LevelVlna[] vlny)
        {
            var plan = new List<PolozkaPlanuVln>();
            float startDalsiVlny = 0;
            foreach (var vlna in vlny)
            {
                startDalsiVlny += vlna.OdstupZacatkuVlny;
                float maxCas = startDalsiVlny;
                foreach (var jednotka in vlna.Jednotky)
                {
                    float casDalsiJednotky = startDalsiVlny + jednotka.Odstup;
                    for (int i = 0; i < jednotka.Pocet; i++)
                    {
                        if (i > 0)
                            casDalsiJednotky += jednotka.Rozestup;
                        plan.Add(new PolozkaPlanuVln()
                        {
                            Cas = casDalsiJednotky,
                            Jednotka = jednotka
                        });
                        if (casDalsiJednotky > maxCas)
                            maxCas = casDalsiJednotky;
                    }
                }
                startDalsiVlny = maxCas;
            }
            plan = plan.OrderBy(x => x.Cas).ToList();
            return plan;
        }
    }

    internal class Level
    {
        public int Cislo { get; set; }
        public LevelVlna[] Vlny { get; set; }
        public List<PolozkaPlanuVln> PlanPosilaniVln { get; set; }
        public LevelVez[] Veze { get; set; }
        public LevelMapa Mapa { get; set; }

        public static Level Nacti(XElement eLevel)
        {
            Level level = new Level();
            // Načtení vln
            var vlny = new List<LevelVlna>();
            foreach (var eVlna in eLevel.Element("vlny").Elements())
            {
                var vlna = new LevelVlna();
                vlna.OdstupZacatkuVlny = eVlna.Attribute("odstup") == null ? 0f : (float)eVlna.Attribute("odstup");
                // Načtení jednotek
                var jednotky = new List<LevelVlnaJednotka>();
                foreach (var eJednotka in eVlna.Elements())
                {
                    var jednotka = new LevelVlnaJednotka();
                    jednotka.Typ = (TypNepritele)Enum.Parse(typeof(TypNepritele), eJednotka.Attribute("typ").Value);
                    jednotka.Pocet = (int)eJednotka.Attribute("pocet");
                    jednotka.Rozestup = (float)eJednotka.Attribute("rozestup");
                    jednotka.Zdravi = (float)eJednotka.Attribute("zdravi");
                    jednotka.Sila = (float)eJednotka.Attribute("sila");
                    jednotka.Rychlost = (float)eJednotka.Attribute("rychlost");
                    jednotka.Odstup = eJednotka.Attribute("odstup") == null ? 0f : (float)eJednotka.Attribute("odstup");
                    jednotky.Add(jednotka);
                }
                vlna.Jednotky = jednotky.ToArray();
                vlny.Add(vlna);
            }
            level.Vlny = vlny.ToArray();
            level.PlanPosilaniVln = PolozkaPlanuVln.NactiPlanVln(level.Vlny);

            // Načtení věží
            var veze = new List<LevelVez>();
            foreach (var eVez in eLevel.Element("veze").Elements())
            {
                var vez = new LevelVez();
                vez.Typ = (TypVeze)Enum.Parse(typeof(TypVeze), eVez.Attribute("typ").Value);
                vez.Pocet = (int)eVez.Attribute("pocet");
                vez.MaxUroven = (short)eVez.Attribute("maxUroven");
                veze.Add(vez);
            }
            level.Veze = veze.ToArray();

            // Načtení mapy
            level.Mapa = LevelMapa.NactiMapu(eLevel.Element("mapa"));

            return level;
        }
    }
}
