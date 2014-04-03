using System;
using System.Collections.Generic;
using LogikaGeneracji.PrzetwarzaczeMapy;
using LogikaGeneracji.PrzetwarzaczeMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
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