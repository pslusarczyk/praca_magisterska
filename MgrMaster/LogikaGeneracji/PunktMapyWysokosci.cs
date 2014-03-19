using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public class PunktMapyWysokosci : IPunktMapyWysokosci
   {
      public IEnumerable<IPunkt> Sasiedzi { get; set; }
      public IPunkt NajnizszySasiad { get; set; }
      public Vector3 Pozycja { get; set; }
      public float Wysokosc { get; set; }
   }
}