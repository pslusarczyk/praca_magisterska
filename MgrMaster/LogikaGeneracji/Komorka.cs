using System.Collections.Generic;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
    public interface IKomorka : IPunkt
    {
        IEnumerable<IRog> Rogi { get; set; }
    }

    public class Komorka : IKomorka
    {
        public Komorka(Vector wektorFortunea)
        {
            Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaVector3(wektorFortunea);
            Rogi = new List<IRog>();
        }

        public Vector3 Pozycja { get; set; }

        public IEnumerable<IPunkt> Sasiedzi { get; set; }
        public IEnumerable<IPunkt> NajnizszySasiad { get; set; }
        public IEnumerable<IRog> Rogi { get; set; }
    }
}