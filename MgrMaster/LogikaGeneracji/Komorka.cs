using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
   public interface IKomorka : IPunkt
   {
      IList<IRog> Rogi { get; set; }
      IList<IKomorka> PrzylegleKomorki { get; set; }
      void DodajRogi(IRog pierwszy, IRog drugi);
   }

   public class Komorka : IKomorka
   {
      public Komorka(Vector wektorFortunea)
      {
         Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea);
         Rogi = new List<IRog>();
         PrzylegleKomorki = new List<IKomorka>();
      }

      public IEnumerable<IPunkt> SasiedniePunkty
      {
         get { return Rogi.Select(k => k as IPunkt).Union(PrzylegleKomorki.Select(r => r as IPunkt)); }
         set { throw new System.NotImplementedException("Ta w³aœciwoœæ w danej implementacji jest wyliczana"); }
      }

      public Vector3 Pozycja { get; set; }

      public IList<IKomorka> PrzylegleKomorki { get; set; }
      public IPunkt NajnizszySasiad { get; set; }
      public IList<IRog> Rogi { get; set; }

      public void DodajRogi(IRog pierwszy, IRog drugi)
      {
         if (!Rogi.Contains(pierwszy))
            Rogi.Add(pierwszy);
         if (!Rogi.Contains(drugi))
            Rogi.Add(drugi);
      }
   }
}