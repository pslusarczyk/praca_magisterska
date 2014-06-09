using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class WyrownywaczTerenuWody : BazaPrzetwarzacza
   {
      private readonly float _poziom;

      public WyrownywaczTerenuWody(float poziom = 0f)
      {
         _poziom = poziom;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            if (komorka.Dane.Podloze == Podloze.Woda)
            {
               IList<IPunkt> punkty = komorka.Rogi
                  .Where(r => r.Komorki.All(k => k.Dane.Podloze != Podloze.Ziemia))
                  .Select(r => r.Punkt)
                  .Union(new[] {komorka.Punkt}).ToList();

               foreach (IPunkt punkt in punkty)
                  punkt.Wysokosc = _poziom;
            }
         }
      }
   }
}