using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji
{
   public interface IKomorka
   {
      int Id { get; set; }
      DaneKomorki Dane { get; set; }
      IPunkt Punkt { get; set; }
      bool Skrajna { get; set; }
      IList<IRog> Rogi { get; set; }
      IList<IKomorka> PrzylegleKomorki { get; set; }
      void DodajRogi(IRog pierwszy, IRog drugi);
   }

   [Serializable]
   [XmlRoot]
   [XmlInclude(typeof(Punkt))]
   public class Komorka : IKomorka
   {
      public Komorka()
      {
         Id = new TworcaIdKomorek().UtworzId();
         Punkt = new Punkt {Pozycja = new MojVector3()};
         Rogi = new List<IRog>();
         PrzylegleKomorki = new List<IKomorka>();
         Dane = new DaneKomorki(); // todo jeœliby wprowadziæ etap wstêpny etap przetwarzania dzia³aj¹cy na tym polu, to mo¿naby wywaliæ tê inicjalizacjê ¿eby by³o wiadomo, czy okreœlono dane czy nie
      }

      public Komorka(Vector wektorFortunea) : this()
      {
         Punkt.Pozycja = NarzedziaPrzetwarzaniaFortunea.VectorNaMojVector3(wektorFortunea);
      }

      public int Id { get; set; }
      public bool Skrajna { get; set; }
      [XmlIgnore]public IList<IKomorka> PrzylegleKomorki { get; set; }
      [XmlIgnore]
      public HashSet<OdcinekRzeki> OdcinkiRzek { get; set; }
      [XmlIgnore]
      public IPunkt NajnizszySasiad { get; set; }
      [XmlIgnore]
      public DaneKomorki Dane { get; set; }
      [XmlIgnore]
      public IPunkt Punkt { get; set; }
      [XmlIgnore]public IList<IRog> Rogi { get; set; }

      public void DodajRogi(IRog pierwszy, IRog drugi)
      {
         if (!Rogi.Contains(pierwszy))
         {
            Rogi.Add(pierwszy);
            Punkt.Sasiedzi.Add(pierwszy.Punkt);
         }
         if (!Rogi.Contains(drugi))
         {
            Rogi.Add(drugi);
            Punkt.Sasiedzi.Add(drugi.Punkt);
         }
      }
   }

   [XmlRoot]
   public class DaneKomorki
   {
      public Podloze? Podloze { get; set; }
      public TypKomorki? Typ { get; set; }
      public BrzeznoscKomorki? Brzeznosc { get; set; }
      public float Temperatura { get; set; }
      public float Wilgotnosc { get; set; }
      public Biom? Biom { get; set; }
   }
}