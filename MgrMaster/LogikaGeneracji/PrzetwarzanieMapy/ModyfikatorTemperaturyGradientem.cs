using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorTemperaturyGradientem : BazaPrzetwarzacza
   {
      private KonfiguracjaModyfikatoraTemperaturyGradientem _konfig;

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

      public override void Przetwarzaj(IMapa mapa)
      {

      }
   }
}