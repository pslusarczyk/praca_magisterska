using System.Collections.Generic;

namespace LogikaGeneracji
{
   public class MapaWysokosci : IZbiorPunktowMapyWysokosci
   {
      public IEnumerable<IPunktMapyWysokosci> Punkty { get; set; }
   }

   public interface IZbiorPunktowMapyWysokosci
   {
      IEnumerable<IPunktMapyWysokosci> Punkty { get; set; } 
   }
}