using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorTemperaturyGradientem : IPrzetwarzaczMapy
   {
      private KonfiguracjaModyfikatoraTemperaturyGradientem _konfig;

      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public KonfiguracjaModyfikatoraTemperaturyGradientem Konfiguracja
      {
         get
         {
            return _konfig ?? (_konfig =
               new KonfiguracjaModyfikatoraTemperaturyGradientem
               {
                  
               });
         }
         set { _konfig = value; }
      }

      public void Przetwarzaj(IMapa mapa)
      {

      }
   }
}