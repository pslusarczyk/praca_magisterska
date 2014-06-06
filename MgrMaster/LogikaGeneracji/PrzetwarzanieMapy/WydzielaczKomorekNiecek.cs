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
            if(LezyWNiecce(komorka))
               mapa.KomorkiNiecki.Add(komorka);
         }
      }

      private static bool LezyWNiecce(IKomorka komorka)
      {
         return komorka.Dane.Brzeznosc == BrzeznoscKomorki.OtwartyLad 
                                       && 
                ((komorka.Punkt.Nastepnik == null) || PosiadaWyzszePrzylegleKomorki(komorka));
      }

      private static bool PosiadaWyzszePrzylegleKomorki(IKomorka komorka)
      {
         return komorka.PrzylegleKomorki.Any(
                                             p => p.Dane.Brzeznosc == BrzeznoscKomorki.OtwartyLad 
                                                  && p.Punkt.Wysokosc > komorka.Punkt.Wysokosc
                                            );
      }
   }
}