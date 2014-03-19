using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      IEnumerable<IPunkt> Sasiedzi { get; set; }
      Vector3 Pozycja { get; set; }
   }

   public interface IPunktMapyWysokosci : IPunkt
   {
      float Wysokosc { get; set; }
      IPunkt NajnizszySasiad { get; set; }
   }
}