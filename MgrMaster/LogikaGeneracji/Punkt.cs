using System.Collections.Generic;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      int Id { get; set; }
      IList<IPunkt> Sasiedzi { get; set; }
      Vector3 Pozycja { get; set; }
      float Wysokosc { get; set; }
      IPunkt Nastepnik { get; set; }
   }

   public class Punkt : IPunkt
   {
      public Punkt()
      {
         Id = new TworcaIdPunktow().UtworzId();
         Sasiedzi = new List<IPunkt>();
         Wysokosc = 0f;
      }

      public int Id { get; set; }
      public IList<IPunkt> Sasiedzi { get; set; }
      public Vector3 Pozycja { get; set; }
      public float Wysokosc { get; set; }
      public IPunkt Nastepnik { get; set; }
   }


}