using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorTemperaturyNaPodstawieWysokosci : IPrzetwarzaczMapy
   {
      private KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci _konfig;

      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci Konfiguracja { 
         get
         {
            return _konfig ?? (_konfig =
               new KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci
               {
                  Baza = 20f,
                  ZmianaNaJednostke = -3f
               });
         }
         set { _konfig = value; } }

      public void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            komorka.Dane.Temperatura +=
               Konfiguracja.Baza + komorka.Punkt.Wysokosc*Konfiguracja.ZmianaNaJednostke;
         }
      }
   }
}