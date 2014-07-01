using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorBiomow : BazaPrzetwarzacza
   {
      private readonly KonfigAktualizatoraBiomow _konfiguracja;

      public AktualizatorBiomow(KonfigAktualizatoraBiomow konfig)
      {
         _konfiguracja = konfig;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki.Where(k => k.Dane.Podloze == Podloze.Ziemia))
         {
            komorka.Dane.Biom = _konfiguracja.PobierzBiom(komorka.Dane.Temperatura, komorka.Dane.Wilgotnosc);
         }
      }
   }
}