using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      IList<IPunkt> Sasiedzi { get; set; }
      Vector3 Pozycja { get; set; }
      float Wysokosc { get; set; }
      IPunkt Nastepnik { get; set; }
   }

   public class Punkt : IPunkt
   {
      public Punkt()
      {
         Sasiedzi = new List<IPunkt>();
         Wysokosc = 0f;
      }

      public IList<IPunkt> Sasiedzi { get; set; }
      public Vector3 Pozycja { get; set; }
      public float Wysokosc { get; set; }
      public IPunkt Nastepnik { get; set; }
   }
}