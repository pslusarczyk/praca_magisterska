using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class GeneratorRzeki : IGeneratorRzeki
   {
      private const float GruboscJednostkowa = 1f;

      private float _aktualnaGrubosc;
      private IMapa _mapa;
      private IList<IOdcinekRzeki> _odcinki;

      public Random Random { get; set; }
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public IPunkt PunktPoczatkowy { get; set; }
      public bool? UdaloSieUtworzyc { get; set; }

      public GeneratorRzeki(IPunkt punktPoczatkowy)
      {
         PunktPoczatkowy = punktPoczatkowy;
      }

      public void Przetwarzaj(IMapa mapa)
      {
         _mapa = mapa;
         _aktualnaGrubosc = GruboscJednostkowa;
         _odcinki = new List<IOdcinekRzeki>();

         SprobujUtworzycRzeke(PunktPoczatkowy);
      }

      // Pilne Poprawiæ pod k¹tem zagnie¿d¿eñ.
      private void SprobujUtworzycRzeke(IPunkt PunktPoczatkowy)
      {
         IPunkt aktualnyPunkt = PunktPoczatkowy;
         IPunkt nastepnyPunkt = aktualnyPunkt.Nastepnik;

         while (nastepnyPunkt != null)
         {
            _odcinki.Add(new OdcinekRzeki
            {
               Grubosc = GruboscJednostkowa,
               PunktA = aktualnyPunkt,
               PunktB = nastepnyPunkt
            });
            IRzeka kolizyjnaRzeka = _mapa.Rzeki.FirstOrDefault(rz => rz.Odcinki.Any(o => o.PunktA == nastepnyPunkt));
            if (kolizyjnaRzeka != null)
            {
               bool kolidujacaJestDluzsza = _odcinki.Count < kolizyjnaRzeka.DlugoscDoPunktu(nastepnyPunkt);
               if (kolidujacaJestDluzsza)
               {
                  PogrubRzekeOdPunktu(kolizyjnaRzeka, nastepnyPunkt, _aktualnaGrubosc);
                  aktualnyPunkt = nastepnyPunkt;
                  break;
               }
               else
               {
                  float gruboscDoDodania = kolizyjnaRzeka.Odcinki.First(o => o.PunktA == nastepnyPunkt).Grubosc;
                  _aktualnaGrubosc += gruboscDoDodania;
                  IList<IOdcinekRzeki> wycinek = WytnijCzescRzekiOdPunktu(kolizyjnaRzeka, nastepnyPunkt);
                  _odcinki = _odcinki.Concat(wycinek).ToList();
                  aktualnyPunkt = _odcinki.Last().PunktB;
                  nastepnyPunkt = aktualnyPunkt.Nastepnik;
                  continue;
               }
            }

            aktualnyPunkt = nastepnyPunkt;
            nastepnyPunkt = aktualnyPunkt.Nastepnik;
         }

         if (KoncoweMiejsceJestNaBrzegu(_mapa, aktualnyPunkt))
         {
            UdaloSieUtworzyc = true;
            _mapa.Rzeki.Add(new Rzeka {Odcinki = _odcinki});
         }
         else
         {
            UdaloSieUtworzyc = false;
         }
      }

      private IList<IOdcinekRzeki> WytnijCzescRzekiOdPunktu(IRzeka rzeka, IPunkt punkt)
      {
         int indeks = rzeka.DlugoscDoPunktu(punkt);
         var wycinek = rzeka.Odcinki.Skip(indeks).ToList();
         rzeka.Odcinki = rzeka.Odcinki.Take(indeks).ToList();
         return wycinek;
      }

      private void PogrubRzekeOdPunktu(IRzeka rzeka, IPunkt punkt, float aktualnaGrubosc)
      {
         int indeksPunktu = rzeka.DlugoscDoPunktu(punkt);
         foreach (var odcinekRzeki in rzeka.Odcinki.Skip(indeksPunktu))
         {
            odcinekRzeki.Grubosc += aktualnaGrubosc;
         }
      }

      private IOdcinekRzeki SplynDalej(IOdcinekRzeki aktualnyOdcinek)
      {
         IOdcinekRzeki istniejaceNastepne = NastepneMiejsceANalezaceJuzDoInnejRzeki(aktualnyOdcinek);

         var nastepneMiejsce = istniejaceNastepne ?? new OdcinekRzeki
         {
            Grubosc = _aktualnaGrubosc,
            PunktB = aktualnyOdcinek.PunktA.Nastepnik
         };
         _odcinki.Add(nastepneMiejsce);

         if (istniejaceNastepne != null)
         {
            ZmodyfikujRzekêNaJak¹Natrafi³eœAPozaTymZakoñczyæTrzebaTylkoWtedyKiedyJestD³u¿sza();
            aktualnyOdcinek = null;
         }
         else aktualnyOdcinek = nastepneMiejsce;

         return aktualnyOdcinek;
      }

      private void ZmodyfikujRzekêNaJak¹Natrafi³eœAPozaTymZakoñczyæTrzebaTylkoWtedyKiedyJestD³u¿sza()
      {
         throw new System.NotImplementedException();
      }

      private IOdcinekRzeki NastepneMiejsceANalezaceJuzDoInnejRzeki(IOdcinekRzeki aktualnyOdcinek)
      {
         return _mapa.Rzeki.SelectMany(rz => rz.Odcinki).FirstOrDefault(m => m.PunktA == aktualnyOdcinek.PunktB.Nastepnik);
      }

      private static bool KoncoweMiejsceJestNaBrzegu(IMapa mapa, IPunkt aktualnyPunkt)
      {
         return mapa.Rogi.Any(r => r.Punkt == aktualnyPunkt && r.Dane.Brzeznosc == BrzeznoscRogu.Brzeg);
      }
   }
}