using System;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class WydzielaczKomorekNiecek : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            if(komorka.Dane.Brzeznosc == null)
               throw new InvalidOperationException("Komórka musi posiadaæ brze¿noœæ, " +
                                                   "by móc sprawdziæ, czy jest nieck¹");
            if(!komorka.Skrajna && LezyWNiecce(komorka))
               mapa.KomorkiNiecki.Add(komorka);
         }
      }

      private static bool LezyWNiecce(IKomorka komorka)
      {
         return komorka.Dane.Brzeznosc == BrzeznoscKomorki.OtwartyLad 
                                       && 
                ((komorka.Punkt.Nastepnik == null) || WszystkiePrzylegleKomorkiSaWyzej(komorka));
      }

      private static bool WszystkiePrzylegleKomorkiSaWyzej(IKomorka komorka)
      {
         return komorka.PrzylegleKomorki.All(
                                             p => p.Dane.Brzeznosc == BrzeznoscKomorki.OtwartyLad 
                                                  && p.Punkt.Wysokosc > komorka.Punkt.Wysokosc
                                            );
      }
   }
}