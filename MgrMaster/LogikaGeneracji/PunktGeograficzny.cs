using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public class PunktGeograficzny : IPunktGeograficzny
   {
      public IEnumerable<IPunkt> SasiedniePunkty { get; set; }
      public IPunkt NajnizszySasiad { get; set; }
      public Vector3 Pozycja { get; set; }
      public float Wysokosc { get; set; }
   }
}