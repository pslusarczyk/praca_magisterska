using System.Collections.Generic;
using LogikaGeneracji.PrzetwarzaczeMapy;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
   public interface IKomorka
   {
      DaneKomorki Dane { get; set; }
      IPunkt Punkt { get; set; }
      IList<IRog> Rogi { get; set; }
      IList<IKomorka> PrzylegleKomorki { get; set; }
      void DodajRogi(IRog pierwszy, IRog drugi);
   }

   public class Komorka : IKomorka
   {
      public Komorka()
      {
         Punkt = new Punkt {Pozycja = new Vector3()};
         Rogi = new List<IRog>();
         PrzylegleKomorki = new List<IKomorka>();
         Dane = new DaneKomorki(); // todo jeœliby wprowadziæ etap wstêpny etap przetwarzania dzia³aj¹cy na tym polu, to mo¿naby wywaliæ tê inicjalizacjê ¿eby by³o wiadomo, czy okreœlono dane czy nie
      }

      public Komorka(Vector wektorFortunea) : this()
      {
         Punkt.Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea);
      }

      public IList<IKomorka> PrzylegleKomorki { get; set; }
      public IPunkt NajnizszySasiad { get; set; }
      public DaneKomorki Dane { get; set; }
      public IPunkt Punkt { get; set; }
      public IList<IRog> Rogi { get; set; }

      public void DodajRogi(IRog pierwszy, IRog drugi)
      {
         if (!Rogi.Contains(pierwszy))
         {
            Rogi.Add(pierwszy);
            Punkt.Sasiedzi.Add(pierwszy.Punkt);
         }
         if (!Rogi.Contains(drugi))
         {
            Rogi.Add(drugi);
            Punkt.Sasiedzi.Add(drugi.Punkt);
         }
      }
   }

   public class DaneKomorki
   {
      public Podloze? Podloze { get; set; }
      public TypKomorki? Typ { get; set; }
      public BrzeznoscKomorki? Brzeznosc { get; set; }

   }
}