using System.Collections.Generic;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
   public interface IRog
   {
      int Id { get; set; }
      DaneRogu Dane { get; set; }
      IPunkt Punkt { get; set; }
      IList<IKomorka> Komorki { get; set; }
      IList<IRog> BliskieRogi { get; set; }
      void DodajKomorki(IKomorka lewa, IKomorka prawa);
   }

   public class Rog : IRog
   {
      public Rog()
      {
         Id = new TworcaIdRogow().UtworzId();
         Komorki = new List<IKomorka>();
         BliskieRogi = new List<IRog>();
         Dane = new DaneRogu(); // todo jak w komórkach: jeœliby wprowadziæ etap wstêpny etap przetwarzania dzia³aj¹cy na tym polu, to mo¿naby wywaliæ tê inicjalizacjê ¿eby by³o wiadomo, czy okreœlono dane czy nie
      }

      public Rog(Vector wektorFortunea) : this()
      {
         Punkt = new Punkt { Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea) };
      }

      public int Id { get; set; }
      public IPunkt NajnizszySasiad { get; set; }
      public DaneRogu Dane { get; set; }
      public IPunkt Punkt { get; set; }
      public IList<IKomorka> Komorki { get; set; }
      public IList<IRog> BliskieRogi { get; set; }

      public void DodajKomorki(IKomorka lewa, IKomorka prawa)
      {
         if (!Komorki.Contains(lewa))
         {
            Komorki.Add(lewa);
            Punkt.Sasiedzi.Add(lewa.Punkt);
         }
         if (!Komorki.Contains(prawa))
         {
            Komorki.Add(prawa);
            Punkt.Sasiedzi.Add(prawa.Punkt);
         }
      }
   }

   public class DaneRogu
   {
      public BrzeznoscRogu? Brzeznosc { get; set; }
   }
}