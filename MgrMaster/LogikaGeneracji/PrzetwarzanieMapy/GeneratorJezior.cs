using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class GeneratorJezior : BazaPrzetwarzacza
   {
      private readonly int _liczbaJezior;

      public GeneratorJezior(int liczbaJezior)
      {
         _liczbaJezior = liczbaJezior;
      }

      public override void Przetwarzaj(IMapa mapa) // pilne napisaæ testy
      {
         if (_liczbaJezior > mapa.KomorkiNiecki.Count)
            throw new InvalidOperationException(
               String.Format("Mapa zawiera mniej niecek ({0}), ni¿ jest jezior do wygenerowania ({1})!",
                  mapa.KomorkiNiecki.Count, _liczbaJezior));
         var gen = new Random();
         List<IKomorka> nieckiDoObsluzenia = mapa.KomorkiNiecki.OrderBy(n => gen.Next()).Take(_liczbaJezior).ToList();
         nieckiDoObsluzenia.ForEach(n => mapa.KomorkiNiecki.Remove(n));
         foreach (IKomorka komorkaNiecka in nieckiDoObsluzenia)
         {
            komorkaNiecka.Dane.Typ = TypKomorki.Jezioro;
            IKomorka komNiecka = komorkaNiecka;
            var doUjeziorzenia = komorkaNiecka.PrzylegleKomorki
               .Where(p => p.Dane.Brzeznosc == BrzeznoscKomorki.OtwartyLad
                           && !PrzylegaDoKomorkiLezacejNizejNicNiecka(komNiecka, p));
            mapa.KomorkiNiecki.Remove(komorkaNiecka);
            doUjeziorzenia.ToList().ForEach(p => p.Dane.Typ = TypKomorki.Jezioro);
         }
      }

      private bool PrzylegaDoKomorkiLezacejNizejNicNiecka(IKomorka komorkaNiecka, IKomorka komorka)
      {
         return komorka.PrzylegleKomorki.Any(p => p.Punkt.Wysokosc < komorkaNiecka.Punkt.Wysokosc);
      }
   }
}