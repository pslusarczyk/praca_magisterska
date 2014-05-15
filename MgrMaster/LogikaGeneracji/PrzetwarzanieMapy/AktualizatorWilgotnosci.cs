using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class AktualizatorWilgotnosci : IPrzetwarzaczMapy
   {
      private HashSet<IKomorka> _odwiedzone;
      private HashSet<IKomorka> _oczekujace;
      private Dictionary<IKomorka, int> _glebokosci;
      public IPrzetwarzaczMapy Nastepnik { get; set; }
      public int GlebokoscPrzeszukiwania { get; set; }
      public float WartoscJeziora { get; set; }
      public float WartoscRzeki { get; set; }
      public float WartoscMorza { get; set; }
      public float MnoznikWartosci { get; set; }

      public void Przetwarzaj(IMapa mapa)
      {
         foreach (IKomorka modyfikowana in mapa.Komorki.Where(k => k.Dane.Podloze == Podloze.Ziemia))
         {
            _odwiedzone = new HashSet<IKomorka>();
            _oczekujace = new HashSet<IKomorka>{modyfikowana};
            _glebokosci = new Dictionary<IKomorka, int> {{modyfikowana, 1}};
            while (_oczekujace.Any())
            {
               IKomorka aktualna = _oczekujace.First();
               _oczekujace.Remove(aktualna);
               DodajSasiadow(aktualna); 
               Odwiedz(modyfikowana, aktualna, _glebokosci[aktualna]);
            }
         }
      }

      private void DodajSasiadow(IKomorka aktualna)
      {
         foreach (IKomorka sasiad in aktualna.PrzylegleKomorki.Where(
            p => !_oczekujace.Contains(p) && !_odwiedzone.Contains(p)
            ))
         {
            UstawGlebokoscIDodajDoOczekujacychJesliTrzeba(sasiad, aktualna);
         }
      }

      private void UstawGlebokoscIDodajDoOczekujacychJesliTrzeba(IKomorka sasiad, IKomorka aktualna)
      {
         _glebokosci[sasiad] = _glebokosci[aktualna] + 1;
         if (_glebokosci[sasiad] <= GlebokoscPrzeszukiwania)
         {
            _oczekujace.Add(sasiad);
         }
      }

      private void Odwiedz(IKomorka modyfikowana, IKomorka aktualna, int glebokosc)
      {
         float mnoznik = Mathf.Pow(MnoznikWartosci, glebokosc-1);
         float suma = 0f;
         if (aktualna.Dane.Typ == TypKomorki.Jezioro)
            suma += WartoscJeziora;
         if (aktualna.Dane.Typ == TypKomorki.Morze)
            suma += WartoscMorza;
         if (aktualna.Punkt.ZawieraRzeke || aktualna.Rogi.Any(r => r.Punkt.ZawieraRzeke))
            suma += WartoscRzeki;

         modyfikowana.Dane.Wilgotnosc
            += suma*mnoznik;

         _odwiedzone.Add(aktualna);
      }
   }
}

