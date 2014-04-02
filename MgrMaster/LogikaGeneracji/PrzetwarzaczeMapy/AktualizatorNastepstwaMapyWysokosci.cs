using System.Linq;

namespace LogikaGeneracji.PrzetwarzaczeMapy
{
   public class AktualizatorNastepstwaMapyWysokosci : IPrzetwarzaczMapy
   {
      public virtual void Przetwarzaj(IMapa mapa)
      {
         foreach (var punktGeograficzny in mapa.Punkty)
         {
            if (!punktGeograficzny.Sasiedzi.Any(s => s.Wysokosc < punktGeograficzny.Wysokosc))
               continue;

            float minimalnaWysokosc = punktGeograficzny.Sasiedzi.Min(s => s.Wysokosc);
            punktGeograficzny.Nastepnik = punktGeograficzny.Sasiedzi
               .First(s => s.Wysokosc == minimalnaWysokosc);
         }
      }
   }
}