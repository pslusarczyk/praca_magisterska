using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
    public class PrzetwarzaczGrafu
    {
        public static object UtworzKomorki(HashSet<VoronoiEdge> krawedzieWoronoja)
        {
            throw new Exception();
        }

        public static List<Dwukrawedz> NaDwukrawedzie(HashSet<VoronoiEdge> krawedzieWoronoja)
        {
            throw new NotImplementedException();
        }
    }

    public class Dwukrawedz
    {
        public Komorka Lewa { get; set; }
        public Komorka Prawa { get; set; }
        public Rog Pierwszy { get; set; }
        public Rog Drugi { get; set; }
    }
}