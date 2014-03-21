using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace LogikaGeneracji
{
   public class MapaGeograficzna : IZbiorPunktowGeograficznych
   {
      public IEnumerable<IPunktGeograficzny> PunktyGeograficzne { get; set; }
      public IZbiorPunktow MapaProsta { get; set; }

      public void UstawPunktomSasiedztwa()
      {
         foreach (var punktGeograficzny in PunktyGeograficzne)
         {
            IPunktGeograficzny geograficzny = punktGeograficzny; // Linq utworzy³ dla bezpieczeñstwa
            punktGeograficzny.Sasiedzi = PunktyGeograficzne.Where(
               s => geograficzny.Punkt.Sasiedzi.Contains(s.Punkt));
         }
      }

      public void ZastosujModyfikatorWysokosci(IModyfikatorWysokosci modyfikator)
      {
         modyfikator.ModyfikujMape(this); // todo sprawdziæ czy to nie zale¿noœæ cykliczna
      }
   }

   public interface IZbiorPunktowGeograficznych
   {
      IEnumerable<IPunktGeograficzny> PunktyGeograficzne { get; set; }
      void UstawPunktomSasiedztwa();
   }
}