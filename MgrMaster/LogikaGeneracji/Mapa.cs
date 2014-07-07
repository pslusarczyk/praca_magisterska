using System;
using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji
{

   public interface IMapa
   {
      IEnumerable<IPunkt> Punkty { get; }
      List<Dwukrawedz> Dwukrawedzie { get; set; }
      HashSet<IKomorka> Komorki { get; set; }
      HashSet<IRog> Rogi { get; set; }
      void ZastosujPrzetwarzanie(IPrzetwarzaczMapy przetwarzacz); // todo jak ni¿ej
      List<IPrzetwarzaczMapy> ZastosowanePrzetwarzacze { get; set; } // todo cykliczne referencje, ale przy bardzo prostej interakcji – mo¿e tak byæ?
      IList<IRzeka> Rzeki { get; set; }
      IList<IKomorka> KomorkiNiecki { get; set; }

      void UstawPunktomSasiedztwa();
   }

   [Serializable]
   public class Mapa : IMapa
   {
      [SerializeField]
      private List<Dwukrawedz> _dwukrawedzie;
      [SerializeField]
      private HashSet<IKomorka> _komorki;
      [SerializeField] // pilne HashSet nie jest serializowany przez Unity! w tym celu trzeba go podszyæ list¹
      private HashSet<IRog> _rogi;
      [SerializeField]
      private List<IPrzetwarzaczMapy> _zastosowanePrzetwarzacze;
      [SerializeField]
      private IList<IRzeka> _rzeki;
      [SerializeField]
      private IList<IKomorka> _komorkiNiecki;
      public bool ZakonczonoTworzenie { get; set; }

      public List<Dwukrawedz> Dwukrawedzie
      {
         get { return _dwukrawedzie; }
         set { _dwukrawedzie = value; }
      }

      public HashSet<IKomorka> Komorki
      {
         get { return _komorki; }
         set { _komorki = value; }
      }

      public HashSet<IRog> Rogi
      {
         get { return _rogi; }
         set { _rogi = value; }
      }

      public List<IPrzetwarzaczMapy> ZastosowanePrzetwarzacze
      {
         get { return _zastosowanePrzetwarzacze; }
         set { _zastosowanePrzetwarzacze = value; }
      }

      public IList<IRzeka> Rzeki
      {
         get { return _rzeki; }
         set { _rzeki = value; }
      }

      public IList<IKomorka> KomorkiNiecki
      {
         get { return _komorkiNiecki; }
         set { _komorkiNiecki = value; }
      }

      public void ZastosujPrzetwarzanie(IPrzetwarzaczMapy przetwarzacz)
      {
         przetwarzacz.Przetwarzaj(this);
         ZastosowanePrzetwarzacze.Add(przetwarzacz);
         if(przetwarzacz.Nastepnik != null)
            ZastosujPrzetwarzanie(przetwarzacz.Nastepnik);
      }

      public Mapa()
      {
         ZastosowanePrzetwarzacze = new List<IPrzetwarzaczMapy>();
         Rzeki = new List<IRzeka>();
         KomorkiNiecki = new List<IKomorka>();
         ZakonczonoTworzenie = false;
      }

      public void UstawPunktomSasiedztwa()
      {
         foreach (var punkt in Punkty)
         {
            IPunkt p = punkt; // Linq utworzy³ dla bezpieczeñstwa
            punkt.Sasiedzi = Punkty.Where(
               s => p.Sasiedzi.Contains(s)).ToList();
         }
      }

      public virtual IEnumerable<IPunkt> Punkty // wydajnoœæ: ta w³aœciwoœæ jest obliczana za ka¿dym razem
      {
         get
         {
            return new List<IPunkt>(
               Rogi.Select(r => r.Punkt))
               .Union(
               Komorki.Select(k => k.Punkt)
               ).ToList();
         }
      }
   }
}