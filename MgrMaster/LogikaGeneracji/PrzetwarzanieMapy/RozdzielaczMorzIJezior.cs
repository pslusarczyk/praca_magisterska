using System;
using System.Collections.Generic;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class RozdzielaczMorzIJezior : BazaPrzetwarzacza
   {
      private readonly IEnumerable<IKomorka> _inicjatorzyPowodzi;
      private HashSet<IKomorka> _zalane;

      public RozdzielaczMorzIJezior(IKomorka inicjator)
      {
         _inicjatorzyPowodzi = new IKomorka[]{inicjator};
      }

      public RozdzielaczMorzIJezior(IEnumerable<IKomorka> inicjatorzy)
      {
         _inicjatorzyPowodzi = inicjatorzy;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         _zalane = new HashSet<IKomorka>();
         foreach (IKomorka inicjator in _inicjatorzyPowodzi)
         {
            if (!mapa.Komorki.Contains(inicjator))
               throw new ArgumentException("Mapa nie zawiera podanego w konstruktorze inicjatora powodzi");
            Rozlewaj(inicjator);
         }


         foreach (IKomorka komorka in mapa.Komorki)
         {
            PrzypiszKomorceTyp(komorka);
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
   }
}