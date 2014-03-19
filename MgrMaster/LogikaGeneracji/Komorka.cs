using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
    public interface IKomorka : IPunkt
    {
        IList<IRog> Rogi { get; set; }
        void DodajRogi(IRog pierwszy, IRog drugi);
        IList<IKomorka> PrzylegleKomorki { get; set; }
    }

   public class Komorka : IKomorka
   {
        public Komorka(Vector wektorFortunea)
        {
            Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea);
            Rogi = new List<IRog>();
            Sasiedzi = new List<IPunkt>();
            PrzylegleKomorki = new List<IKomorka>();
        }

        public Vector3 Pozycja { get; set; }

        public IList<IKomorka> PrzylegleKomorki { get; set; }
        public IList<IPunkt> Sasiedzi { get; set; }
        public IPunkt NajnizszySasiad { get; set; }
        public IList<IRog> Rogi { get; set; }
        public void DodajRogi(IRog pierwszy, IRog drugi)
        {
            if(!Rogi.Contains(pierwszy))
                Rogi.Add(pierwszy);
            if(!Rogi.Contains(drugi))
                Rogi.Add(drugi);
        }
    }
}