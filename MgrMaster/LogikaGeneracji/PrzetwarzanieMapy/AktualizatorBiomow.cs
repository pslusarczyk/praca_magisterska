using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorBiomow : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public KonfigAktualizatoraBiomow Konfiguracja { get; set; }

      public void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            komorka.Dane.Biom = Konfiguracja.PobierzBiom(komorka.Dane.Temperatura, komorka.Dane.Wilgotnosc);
         }
      }
   }
}