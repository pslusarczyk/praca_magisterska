using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji
{
   public interface IZbiorPunktowGeograficznych
   {
      IEnumerable<IPunktGeograficzny> PunktyGeograficzne { get; set; }
      void UstawPunktomSasiedztwa();
      void ZastosujModyfikatorWysokosci(IModyfikatorWysokosci modyfikator);
   }

   public class MapaGeograficzna : IZbiorPunktowGeograficznych, IZbiorKomorekGeograficznych
   {
      public IEnumerable<IPunktGeograficzny> PunktyGeograficzne { get; set; }
      public IEnumerable<IKomorkaGeograficzna> KomorkiGeograficzne { get; set; }
      public IZbiorPunktow MapaProsta { get; set; }

      public void UstawPunktomSasiedztwa()
      {
         foreach (var punktGeograficzny in PunktyGeograficzne)
         {
            IPunktGeograficzny geograficzny = punktGeograficzny; // Linq utworzy� dla bezpiecze�stwa
            punktGeograficzny.Sasiedzi = PunktyGeograficzne.Where(
               s => geograficzny.Punkt.Sasiedzi.Contains(s.Punkt));
         }
      }

      public void ZastosujModyfikatorWysokosci(IModyfikatorWysokosci modyfikator)
      {
         modyfikator.ModyfikujMape(this); // todo zale�no�� cykliczna � trzeba co� z tym robi�?
      }

      public void UstawKomorkomSasiedztwa()
      {
         throw new System.NotImplementedException();
      }
   }
}