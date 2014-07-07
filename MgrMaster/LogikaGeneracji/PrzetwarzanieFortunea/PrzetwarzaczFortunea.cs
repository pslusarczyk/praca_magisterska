using System;
using System.Collections.Generic;
using System.Linq;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji.PrzetwarzanieFortunea
{
   public class PrzetwarzaczFortunea
   {
      private Dictionary<Vector, IKomorka> _komorkiZVectorami;
      private Dictionary<Vector, IRog> _rogiZVectorami;
      private readonly Mapa _mapa = new Mapa();

      public Mapa Mapa
      {
         get { return _mapa; }
      }


      public IMapa Przetwarzaj(HashSet<VoronoiEdge> krawedzieWoronoja)
      {
         _komorkiZVectorami = new Dictionary<Vector, IKomorka>();
         _rogiZVectorami = new Dictionary<Vector, IRog>();
         Mapa.Dwukrawedzie = krawedzieWoronoja.Select(woro => UtworzDwukrawedz(woro)).ToList();
         UstawKomorkomPrzylegle();
         UstawRogomBliskich();
         Mapa.ZakonczonoTworzenie = true;
         return Mapa;
      }

      private Dwukrawedz UtworzDwukrawedz(VoronoiEdge woro)
      {
         var dwukrawedz = new Dwukrawedz();

         UtworzSkladoweDwukrawedzi(woro, dwukrawedz);
         PolaczKomorkiIRogiZDwukrawedzi(dwukrawedz);

         Mapa.Komorki = new HashSet<IKomorka>(_komorkiZVectorami.Values);
         Mapa.Rogi = new HashSet<IRog>(_rogiZVectorami.Values);

         return dwukrawedz;
      }

      private static void PolaczKomorkiIRogiZDwukrawedzi(Dwukrawedz dwukrawedz)
      {
         dwukrawedz.Lewa.DodajRogi(dwukrawedz.Pierwszy, dwukrawedz.Drugi);
         dwukrawedz.Prawa.DodajRogi(dwukrawedz.Pierwszy, dwukrawedz.Drugi);
         dwukrawedz.Pierwszy.DodajKomorki(dwukrawedz.Lewa, dwukrawedz.Prawa);
         dwukrawedz.Drugi.DodajKomorki(dwukrawedz.Lewa, dwukrawedz.Prawa);
      }

      private void UtworzSkladoweDwukrawedzi(VoronoiEdge woro, Dwukrawedz dwukrawedz)
      {
         if (!_komorkiZVectorami.ContainsKey(woro.LeftData))
            _komorkiZVectorami[woro.LeftData] = new Komorka(woro.LeftData)
            {
               Skrajna = woro.LeftData.Skrajny
            };
         dwukrawedz.Lewa = _komorkiZVectorami[woro.LeftData];

         if (!_komorkiZVectorami.ContainsKey(woro.RightData))
            _komorkiZVectorami[woro.RightData] = new Komorka(woro.RightData)
            {
               Skrajna = woro.RightData.Skrajny
            };
         dwukrawedz.Prawa = _komorkiZVectorami[woro.RightData];

         if (!_rogiZVectorami.ContainsKey(woro.VVertexA))
            _rogiZVectorami[woro.VVertexA] = new Rog(woro.VVertexA);
         dwukrawedz.Pierwszy = _rogiZVectorami[woro.VVertexA];

         if (!_rogiZVectorami.ContainsKey(woro.VVertexB))
            _rogiZVectorami[woro.VVertexB] = new Rog(woro.VVertexB);
         dwukrawedz.Drugi = _rogiZVectorami[woro.VVertexB];
      }

      private void UstawKomorkomPrzylegle()
      {
         foreach (Dwukrawedz dwukrawedz in Mapa.Dwukrawedzie)
         {
            if (!dwukrawedz.Lewa.PrzylegleKomorki.Contains(dwukrawedz.Prawa))
            {
               dwukrawedz.Lewa.PrzylegleKomorki.Add(dwukrawedz.Prawa);
            }
            if (!dwukrawedz.Prawa.PrzylegleKomorki.Contains(dwukrawedz.Lewa))
            {
               dwukrawedz.Prawa.PrzylegleKomorki.Add(dwukrawedz.Lewa);
            }
         }
      }

      private void UstawRogomBliskich()
      {
         foreach (Dwukrawedz dwukrawedz in Mapa.Dwukrawedzie)
         {
            if (!dwukrawedz.Pierwszy.BliskieRogi.Contains(dwukrawedz.Drugi))
            {
               dwukrawedz.Pierwszy.BliskieRogi.Add(dwukrawedz.Drugi);
               dwukrawedz.Pierwszy.Punkt.Sasiedzi.Add(dwukrawedz.Drugi.Punkt);
            }
            if (!dwukrawedz.Drugi.BliskieRogi.Contains(dwukrawedz.Pierwszy))
            {
               dwukrawedz.Drugi.BliskieRogi.Add(dwukrawedz.Pierwszy);
               dwukrawedz.Drugi.Punkt.Sasiedzi.Add(dwukrawedz.Pierwszy.Punkt);
            }
         }
      }
   }

   [Serializable]
   public class Dwukrawedz
   {
      public IKomorka Lewa { get; set; }
      public IKomorka Prawa { get; set; }
      public IRog Pierwszy { get; set; }
      public IRog Drugi { get; set; }
   }
}