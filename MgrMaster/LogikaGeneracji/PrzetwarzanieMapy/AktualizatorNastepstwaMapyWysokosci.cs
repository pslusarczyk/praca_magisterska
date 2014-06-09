using System;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using Debug = UnityEngine.Debug;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorNastepstwaMapyWysokosci : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
         ObejscieNaDziwnyRog(mapa);
         
         foreach (var punkt in mapa.Punkty)
         {
            if (PunktNalezyDoRoguBedacegoBrzegiemLubMorzem(mapa, punkt))
               continue;

            if (!punkt.Sasiedzi.Any(
               s => s.Wysokosc <= punkt.Wysokosc
               ))
               continue;

            float minimalnaWysokosc = punkt.Sasiedzi.Min(s => s.Wysokosc);
            punkt.Nastepnik = punkt.Sasiedzi
               .First(s => s.Wysokosc == minimalnaWysokosc);
         }
      }

      private static bool PunktNalezyDoRoguBedacegoBrzegiemLubMorzem(IMapa mapa, IPunkt s)
      {
         return mapa.Rogi.Any(r => r.Punkt == s
            && ((r.Dane.Brzeznosc == BrzeznoscRogu.Brzeg) || (r.Dane.Brzeznosc == BrzeznoscRogu.OtwarteMorze)));
      }

      private static void ObejscieNaDziwnyRog(IMapa mapa) // pilne zlikwidowaæ problem
      {
         IRog winowajca = mapa.Rogi.FirstOrDefault(r => float.IsNaN(r.Punkt.Wysokosc));
         if (winowajca == null)
            throw new Exception("Obejœcie usuwaj¹ce dziwny róg s¹siaduj¹cy ze wszystkimi nie znalaz³o tego rogu!");
         foreach (IPunkt sasiadWinowajcy in winowajca.Punkt.Sasiedzi)
         {
            sasiadWinowajcy.Sasiedzi.Remove(winowajca.Punkt);
         }
         foreach (IRog bliskiWinowajcy in winowajca.BliskieRogi)
         {
            bliskiWinowajcy.BliskieRogi.Remove(winowajca);
         }
         foreach (IKomorka komorkaWinowajcy in winowajca.Komorki)
         {
            komorkaWinowajcy.Rogi.Remove(winowajca);
         }
         mapa.Rogi.Remove(winowajca);
         //Debug.Log(mapa.Punkty.Count(p => float.IsNaN(p.Wysokosc)));
      }
   }
}