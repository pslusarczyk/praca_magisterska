using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji.PrzetwarzaczeMapy
{
   public class AktualizatorBrzeznosciRogow : IPrzetwarzaczMapy
   {
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public void Przetwarzaj(IMapa mapa)
      {
         foreach (IRog rog in mapa.Rogi)
         {
            PrzypiszRogowiBrzeznosc(rog);
         }
      }

      private void PrzypiszRogowiBrzeznosc(IRog rog)
      {
         List<TypKomorki?> typyKomorekTegoRogu = rog.Komorki.Select(k => k.Dane.Typ).ToList();

         if(typyKomorekTegoRogu.Any(t => t == TypKomorki.Morze)
            && (
                  typyKomorekTegoRogu.Any(t => t == TypKomorki.Jezioro) 
                  || typyKomorekTegoRogu.Any(t => t == TypKomorki.Lad)
            ))
            rog.Dane.Brzeznosc = BrzeznoscRogu.Brzeg;

         if(typyKomorekTegoRogu.All(t => t == TypKomorki.Morze))
            rog.Dane.Brzeznosc = BrzeznoscRogu.OtwarteMorze;

         if (typyKomorekTegoRogu.All(t => t == TypKomorki.Lad || t == TypKomorki.Jezioro))
            rog.Dane.Brzeznosc = BrzeznoscRogu.OtwartyLad;

         throw new InvalidOperationException("Nie uda³o siê przypisaæ rogowi brze¿noœci");
      }
   }
}