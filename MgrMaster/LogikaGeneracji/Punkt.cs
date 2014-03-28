using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      IList<IPunkt> Sasiedzi { get; set; }
      Vector3 Pozycja { get; set; }
   }

   public class Punkt : IPunkt
   {
      public Punkt()
      {
         Sasiedzi = new List<IPunkt>();
      }

      public IList<IPunkt> Sasiedzi { get; set; }
      public Vector3 Pozycja { get; set; }
   }

   public interface IPunktTopologiczny
   {
      IPunkt Punkt { get; set; }
      IEnumerable<IPunktTopologiczny> Sasiedzi { get; set; }
      float Wysokosc { get; set; }
      IPunktTopologiczny Nastepnik { get; set; }
   }
}