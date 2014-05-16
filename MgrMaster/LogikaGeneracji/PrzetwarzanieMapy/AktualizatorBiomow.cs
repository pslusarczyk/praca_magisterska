using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorBiomow : BazaPrzetwarzacza
   {
      public KonfigAktualizatoraBiomow Konfiguracja { get; set; }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            komorka.Dane.Biom = Konfiguracja.PobierzBiom(komorka.Dane.Temperatura, komorka.Dane.Wilgotnosc);
         }
      }
   }
}