using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorTemperaturyNaPodstawieWysokosci : BazaPrzetwarzacza
   {
      private KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci _konfig;
      private float _mnoznikTemperatury;

      public ModyfikatorTemperaturyNaPodstawieWysokosci(float mnoznikTemperatury = 1f)
      {
         _mnoznikTemperatury = mnoznikTemperatury;
      }

      public KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci Konfiguracja { 
         get
         {
            return _konfig ?? (_konfig =
               new KonfiguracjaModyfikatoraTemperaturyNaPodstawieWysokosci
               {
                  Baza = 35f,
                  ZmianaNaJednostke = -18f
               });
         }
         set { _konfig = value; } }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            komorka.Dane.Temperatura =
               (Konfiguracja.Baza + komorka.Punkt.Wysokosc*Konfiguracja.ZmianaNaJednostke)
               * _mnoznikTemperatury;
         }
      }
   }
}