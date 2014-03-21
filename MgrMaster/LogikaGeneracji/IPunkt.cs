using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      IEnumerable<IPunkt> Sasiedzi { get; set; }
      Vector3 Pozycja { get; set; }
   }

   public interface IPunktGeograficzny
   {
      IPunkt Punkt { get; set; }
      IEnumerable<IPunktGeograficzny> Sasiedzi { get; set; }
      float Wysokosc { get; set; }
      IPunktGeograficzny Nastepnik { get; set; }
   }
}