using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      IEnumerable<IPunkt> SasiedniePunkty { get; set; }
      Vector3 Pozycja { get; set; }
   }

   public interface IPunktGeograficzny : IPunkt
   {
      float Wysokosc { get; set; }
      IPunkt NajnizszySasiad { get; set; }
   }
}