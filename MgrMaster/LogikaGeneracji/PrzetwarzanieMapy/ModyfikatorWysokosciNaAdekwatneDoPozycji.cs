using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorWysokosciNaAdekwatneDoPozycji : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = punkt.Pozycja.y;
         }
      }
   }
}