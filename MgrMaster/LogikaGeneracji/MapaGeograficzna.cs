using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji
{
   public interface IZbiorPunktowTopologicznych
   {
      IEnumerable<IPunktTopologiczny> PunktyTopologiczne { get; set; }
      IEnumerable<IKomorkaGeograficzna> KomorkiGeograficzne { get; set; } 
      void UstawPunktomSasiedztwa();
      void ZastosujModyfikatorWysokosci(IModyfikatorWysokosci modyfikator);
      void ZastosujRozdzielaczWodyIL¹du(IRozdzielaczWodyIL¹du rozdzielacz);
   }

   public class MapaGeograficzna : IZbiorPunktowTopologicznych, IZbiorKomorekGeograficznych
   {
      public IEnumerable<IPunktTopologiczny> PunktyTopologiczne { get; set; }
      public IEnumerable<IKomorkaGeograficzna> KomorkiGeograficzne { get; set; }
      public IMapaProsta MapaProsta { get; set; }

      public void UstawPunktomSasiedztwa()
      {
         foreach (var punktGeograficzny in PunktyTopologiczne)
         {
            IPunktTopologiczny topologiczny = punktGeograficzny; // Linq utworzy³ dla bezpieczeñstwa
            punktGeograficzny.Sasiedzi = PunktyTopologiczne.Where(
               s => topologiczny.Punkt.Sasiedzi.Contains(s.Punkt));
         }
      }

      public void ZastosujModyfikatorWysokosci(IModyfikatorWysokosci modyfikator)
      {
         modyfikator.ModyfikujMape(this); // todo zale¿noœæ cykliczna – trzeba coœ z tym robiæ?
      }

      public void ZastosujRozdzielaczWodyIL¹du(IRozdzielaczWodyIL¹du rozdzielacz)
      {
         rozdzielacz.PrzetworzMape(this); // todo zale¿noœæ cykliczna – trzeba coœ z tym robiæ? 
      }
   }
}