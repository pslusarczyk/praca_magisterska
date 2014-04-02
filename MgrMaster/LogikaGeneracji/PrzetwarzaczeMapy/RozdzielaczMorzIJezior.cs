using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji.PrzetwarzaczeMapy
{
   public class RozdzielaczMorzIJezior : IPrzetwarzaczMapy
   {
      private readonly IKomorka _inicjatorPowodzi;
      private ISet<IKomorka> _zalane;
      public IPrzetwarzaczMapy Nastepnik { get; set; }

      public RozdzielaczMorzIJezior(IKomorka inicjator)
      {
         _inicjatorPowodzi = inicjator;
      }

      public void Przetwarzaj(IMapa mapa)
      {
         if(!mapa.Komorki.Contains(_inicjatorPowodzi))
            throw new ArgumentException("Mapa nie zawiera podanego w konstruktorze inicjatora powodzi");
         _zalane = new HashSet<IKomorka>();

         Rozlewaj(_inicjatorPowodzi);
         foreach (IKomorka komorka in mapa.Komorki)
         {
            PrzypiszKomorceTyp(komorka);
         }

         foreach (IKomorka komorka in mapa.Komorki)
         {
            PrzypiszKomorceBrzeznosc(komorka);
         }
      }

      private void Rozlewaj(IKomorka komorka)
      {
         _zalane.Add(komorka);
         foreach (var sasiad in komorka.PrzylegleKomorki)
         {
            if (sasiad.Dane.Podloze != Podloze.Woda || _zalane.Contains(sasiad))
               continue;
            Rozlewaj(sasiad);
         }
      }

      private void PrzypiszKomorceTyp(IKomorka komorka)
      {
         if (_zalane.Contains(komorka))
            komorka.Dane.Typ = TypKomorki.Morze;
         else if (komorka.Dane.Podloze == Podloze.Ziemia)
            komorka.Dane.Typ = TypKomorki.Lad;
         else if (komorka.Dane.Podloze == Podloze.Woda)
            komorka.Dane.Typ = TypKomorki.Jezioro;
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