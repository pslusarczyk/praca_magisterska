using LogikaGeneracji.PrzetwarzaczeMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorWysokosciNaAdekwatneDoPozycji : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }

      public virtual void Przetwarzaj(IMapa mapa)
      {
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = punkt.Pozycja.y;
         }
      }
   }
}