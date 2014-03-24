using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public class PunktTopologiczny : IPunktTopologiczny
   {
      public IPunkt Punkt { get; set; }
      public IEnumerable<IPunktTopologiczny> Sasiedzi { get; set; }
      public float Wysokosc { get; set; }
      public IPunktTopologiczny Nastepnik { get; set; }

      public PunktTopologiczny()
      {
         Sasiedzi = new List<IPunktTopologiczny>();
      }
   }
}