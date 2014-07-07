using System.Collections.Generic;
using System.Xml.Serialization;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji
{
   public interface IPunkt
   {
      int Id { get; set; }
      [XmlIgnore]
      IList<IPunkt> Sasiedzi { get; set; }
      MojVector3 Pozycja { get; set; }
      float Wysokosc { get; set; }
      [XmlIgnore]
      IPunkt Nastepnik { get; set; }
      bool ZawieraRzeke { get; set; }
      float WysFiz { get; set; }
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
      public MojVector3 Pozycja { get; set; }
      public float Wysokosc { get; set; }
      public IPunkt Nastepnik { get; set; }
      public bool ZawieraRzeke { get; set; }
      public float WysFiz { get; set; }
   }


}