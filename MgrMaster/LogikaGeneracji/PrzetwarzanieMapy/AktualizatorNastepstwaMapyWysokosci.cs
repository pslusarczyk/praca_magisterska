using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorNastepstwaMapyWysokosci : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }

      public virtual void Przetwarzaj(IMapa mapa)
      {
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
            && (r.Dane.Brzeznosc == BrzeznoscRogu.Brzeg) || (r.Dane.Brzeznosc == BrzeznoscRogu.OtwarteMorze));
      }
   }
}