using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public class PunktGeograficzny : IPunktGeograficzny
   {
      public IPunkt Punkt { get; set; }
      public IEnumerable<IPunktGeograficzny> Sasiedzi { get; set; }
      public float Wysokosc { get; set; }
      public IPunktGeograficzny Nastepnik { get; set; }

      public PunktGeograficzny()
      {
         Sasiedzi = new List<IPunktGeograficzny>();
      }
   }
}