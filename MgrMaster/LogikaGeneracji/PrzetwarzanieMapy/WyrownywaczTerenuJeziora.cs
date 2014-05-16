using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class WyrownywaczTerenuJeziora : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            if (komorka.Dane.Typ == TypKomorki.Jezioro)
            {
               IList<IPunkt> punkty = komorka.Rogi.Select(r => r.Punkt)
                  .Union(new[] {komorka.Punkt}).ToList();

               float minimalnaWysokosc = punkty.Min(p => p.Wysokosc);

               foreach (IPunkt punkt in punkty)
                  punkt.Wysokosc = minimalnaWysokosc;
            }
         }
      }
   }
}