using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class GeneratorRzeki : IGeneratorRzeki
   {
      private const float GruboscJednostkowa = 1f;

      private int   _aktualnaDlugosc;
      private float _aktualnaGrubosc;
      private IMapa _mapa;
      private IList<IMiejsceRzeki> _miejscaSplywu;

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
         _aktualnaDlugosc = 0;
         _aktualnaGrubosc = GruboscJednostkowa;
         _miejscaSplywu = new List<IMiejsceRzeki>();
         IMiejsceRzeki aktualneMiejsce = new MiejsceRzeki
         {
            Punkt = PunktPoczatkowy, DlugoscDotad = _aktualnaDlugosc, Grubosc = _aktualnaGrubosc
         };

         SprobujUtworzycRzeke(_miejscaSplywu, aktualneMiejsce);
      }

      private void SprobujUtworzycRzeke(IList<IMiejsceRzeki> miejscaSplywu, IMiejsceRzeki miejscePoczatkowe)
      {
         IMiejsceRzeki aktualneMiejsce = miejscePoczatkowe;
         miejscaSplywu.Add(aktualneMiejsce);
         while (aktualneMiejsce != null && aktualneMiejsce.Punkt.Nastepnik != null)
         {
            _aktualnaDlugosc += 1;
            aktualneMiejsce = SplynDalej(aktualneMiejsce);
         }

         if (KoncoweMiejsceJestNaBrzegu(_mapa, aktualneMiejsce.Punkt))
         {
            UdaloSieUtworzyc = true;
            _mapa.Rzeki.Add(new Rzeka {MiejscaRzeki = miejscaSplywu});
         }
         else
         {
            UdaloSieUtworzyc = false;
         }
      }

      private IMiejsceRzeki SplynDalej(IMiejsceRzeki aktualneMiejsce)
      {
         IMiejsceRzeki istniejaceNastepne = NastepneMiejsceANalezaceJuzDoInnejRzeki(aktualneMiejsce);
         var nastepneMiejsce = istniejaceNastepne ?? new MiejsceRzeki
         {
            DlugoscDotad = _aktualnaDlugosc,
            Grubosc = _aktualnaGrubosc,
            Punkt = aktualneMiejsce.Punkt.Nastepnik
         };
         _miejscaSplywu.Add(nastepneMiejsce);
         bool zakoncz = istniejaceNastepne != null;
         if (zakoncz)
         {
            ZmodyfikujRzekêNaJak¹Natrafi³eœAPozaTymZakoñczyæTrzebaTylkoWtedyKiedyJestD³u¿sza();
            aktualneMiejsce = null;
         }
         
         else aktualneMiejsce = nastepneMiejsce;
         return aktualneMiejsce;
      }

      private void ZmodyfikujRzekêNaJak¹Natrafi³eœAPozaTymZakoñczyæTrzebaTylkoWtedyKiedyJestD³u¿sza()
      {
         throw new System.NotImplementedException();
      }

      private IMiejsceRzeki NastepneMiejsceANalezaceJuzDoInnejRzeki(IMiejsceRzeki aktualneMiejsce)
      {
         return _mapa.Rzeki.SelectMany(rz => rz.MiejscaRzeki).FirstOrDefault(m => m.Punkt == aktualneMiejsce.Punkt.Nastepnik);
      }

      private static bool KoncoweMiejsceJestNaBrzegu(IMapa mapa, IPunkt aktualnyPunkt)
      {
         return mapa.Rogi.Any(r => r.Punkt == aktualnyPunkt && r.Dane.Brzeznosc == BrzeznoscRogu.Brzeg);
      }
   }
}