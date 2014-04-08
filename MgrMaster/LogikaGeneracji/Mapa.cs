using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieFortunea;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji
{
   public interface IMapa
   {
      IEnumerable<IPunkt> Punkty { get; }
      List<Dwukrawedz> Dwukrawedzie { get; set; }
      HashSet<IKomorka> Komorki { get; set; }
      HashSet<IRog> Rogi { get; set; }
      void ZastosujPrzetwarzanie(IPrzetwarzaczMapy przetwarzacz); // todo jak ni�ej
      List<IPrzetwarzaczMapy> ZastosowanePrzetwarzacze { get; set; } // todo cykliczne referencje, ale przy bardzo prostej interakcji � mo�e tak by�?
      IList<IRzeka> Rzeki { get; set; }

      void UstawPunktomSasiedztwa();
   }

   public class Mapa : IMapa
   {
      public bool ZakonczonoTworzenie { get; set; }
      public List<Dwukrawedz> Dwukrawedzie { get; set; }
      public HashSet<IKomorka> Komorki { get; set; }
      public HashSet<IRog> Rogi { get; set; }
      public List<IPrzetwarzaczMapy> ZastosowanePrzetwarzacze { get; set; }
      public IList<IRzeka> Rzeki { get; set; }

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
         ZakonczonoTworzenie = false;
      }

      public void UstawPunktomSasiedztwa()
      {
         foreach (var punkt in Punkty)
         {
            IPunkt p = punkt; // Linq utworzy� dla bezpiecze�stwa
            punkt.Sasiedzi = Punkty.Where(
               s => p.Sasiedzi.Contains(s)).ToList();
         }
      }

      public virtual IEnumerable<IPunkt> Punkty // wydajno��: ta w�a�ciwo�� jest obliczana za ka�dym razem
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