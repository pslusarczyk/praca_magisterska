using System;
using System.Collections.Generic;
using System.Linq;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
    public class PrzetwarzaczGrafu
    {
        private Dictionary<Vector, IKomorka> _komorkiZVectorami;
        private Dictionary<Vector, IRog> _rogiZVectorami;
        public List<Dwukrawedz> Dwukrawedzie { get; private set; }
        public HashSet<IKomorka> Komorki { get; private set; }
        public HashSet<IRog> Rogi { get; private set; }


        public List<Dwukrawedz> Przetwarzaj(HashSet<VoronoiEdge> krawedzieWoronoja)
        {
            _komorkiZVectorami = new Dictionary<Vector, IKomorka>();
            _rogiZVectorami = new Dictionary<Vector, IRog>();
            Dwukrawedzie = krawedzieWoronoja.Select(woro => UtworzDwukrawedz(woro)).ToList();
            return Dwukrawedzie;
        }

        private Dwukrawedz UtworzDwukrawedz(VoronoiEdge woro)
        {
            var dwukrawedz = new Dwukrawedz();

            if (!_komorkiZVectorami.ContainsKey(woro.LeftData))
                _komorkiZVectorami[woro.LeftData] = new Komorka(woro.LeftData);
            dwukrawedz.Lewa = _komorkiZVectorami[woro.LeftData];

            if (!_komorkiZVectorami.ContainsKey(woro.RightData))
                _komorkiZVectorami[woro.RightData] = new Komorka(woro.RightData);
            dwukrawedz.Prawa = _komorkiZVectorami[woro.RightData];

            if (!_rogiZVectorami.ContainsKey(woro.VVertexA))
                _rogiZVectorami[woro.VVertexA] = new Rog(woro.VVertexA);
            dwukrawedz.Pierwszy = _rogiZVectorami[woro.VVertexA];
            
            if (!_rogiZVectorami.ContainsKey(woro.VVertexB))
                _rogiZVectorami[woro.VVertexB] = new Rog(woro.VVertexB);
            dwukrawedz.Drugi = _rogiZVectorami[woro.VVertexB];

            Komorki = new HashSet<IKomorka>(_komorkiZVectorami.Values);
            Rogi = new HashSet<IRog>(_rogiZVectorami.Values);

            return dwukrawedz;
        }
    }

    

    public class Dwukrawedz
    {
        public IKomorka Lewa { get; set; }
        public IKomorka Prawa { get; set; }
        public IRog Pierwszy { get; set; }
        public IRog Drugi { get; set; }
    }
}