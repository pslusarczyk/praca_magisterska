using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      IEnumerable<IPunkt> Sasiedzi { get; }
      IPunkt NajnizszySasiad { get; set; }
      Vector3 Pozycja { get; set; }
   }
}