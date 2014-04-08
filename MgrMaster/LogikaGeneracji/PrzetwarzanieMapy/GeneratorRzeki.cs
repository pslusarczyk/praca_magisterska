using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class GeneratorRzeki : IGeneratorRzeki
   {
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
         IList<IPunkt> punktySplywu = new List<IPunkt>();
         IPunkt aktualnyPunkt = PunktPoczatkowy;

         punktySplywu.Add(aktualnyPunkt);
         while (aktualnyPunkt.Nastepnik != null)
         {
            punktySplywu.Add(aktualnyPunkt.Nastepnik);
            aktualnyPunkt = aktualnyPunkt.Nastepnik;
         } 
        
         if (KoncowyPunktJestNaBrzegu(mapa, aktualnyPunkt))
         {
            UdaloSieUtworzyc = true;
            mapa.Rzeki.Add(new Rzeka { Punkty = punktySplywu });
         }
         else
         {
            UdaloSieUtworzyc = false;
         }
      }

      private static bool KoncowyPunktJestNaBrzegu(IMapa mapa, IPunkt aktualnyPunkt)
      {
         return mapa.Rogi.Any(r => r.Punkt == aktualnyPunkt && r.Dane.Brzeznosc == BrzeznoscRogu.Brzeg);
      }
   }
}