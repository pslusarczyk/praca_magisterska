namespace LogikaGeneracji.PrzetwarzaczeMapy
{
   public class ModyfikatorWysokosciNaAdekwatneDoPozycji : IPrzetwarzaczMapy
   {
      public virtual void Przetwarzaj(IMapa mapa)
      {
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = punkt.Pozycja.y;
         }
      }
   }
}