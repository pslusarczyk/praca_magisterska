using System.Collections.Generic;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
   public interface IKomorka
   {
      IDaneKomorki Dane { get; set; }
      IPunkt Punkt { get; set; }
      IList<IRog> Rogi { get; set; }
      IList<IKomorka> PrzylegleKomorki { get; set; }
      void DodajRogi(IRog pierwszy, IRog drugi);
   }

   public class Komorka : IKomorka
   {
      public Komorka(Vector wektorFortunea)
      {
         Punkt = new Punkt{Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea)};
         Rogi = new List<IRog>();
         PrzylegleKomorki = new List<IKomorka>();
      }

      public IList<IKomorka> PrzylegleKomorki { get; set; }
      public IPunkt NajnizszySasiad { get; set; }
      public IDaneKomorki Dane { get; set; }
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

   public interface IDaneKomorki
   {
   }
}