using System;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorBrzeznosciKomorek : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka komorka in mapa.Komorki)
         {
            PrzypiszKomorceBrzeznosc(komorka);
         }
      }

      private void PrzypiszKomorceBrzeznosc(IKomorka komorka)
      {
         switch (komorka.Dane.Typ)
         {
            case TypKomorki.Morze:
               if (komorka.PrzylegleKomorki.Any(
                  k => k.Dane.Typ == TypKomorki.Jezioro || k.Dane.Typ == TypKomorki.Lad
                  ))
                  komorka.Dane.Brzeznosc = BrzeznoscKomorki.MorzePrzybrzezne;
               else komorka.Dane.Brzeznosc = BrzeznoscKomorki.OtwarteMorze;
               break;

            case TypKomorki.Lad:
               if (komorka.PrzylegleKomorki.Any(
                  k => k.Dane.Typ == TypKomorki.Morze
                  ))
                  komorka.Dane.Brzeznosc = BrzeznoscKomorki.BrzegMorza;
               break;

            case TypKomorki.Jezioro:
               if (komorka.PrzylegleKomorki.Any(
                  k => k.Dane.Typ == TypKomorki.Morze)
                  )
                  throw new InvalidOperationException("Komórka jeziorna nie powinna s¹siadowaæ z morsk¹!");
               komorka.Dane.Brzeznosc = BrzeznoscKomorki.OtwartyLad;
               break;
            default:
               throw new InvalidOperationException("Komórce bez typu nie mo¿na przypisaæ brze¿noœci!");
         }
      }
   }
}