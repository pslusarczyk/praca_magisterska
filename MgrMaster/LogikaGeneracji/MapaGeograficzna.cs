using System.Collections.Generic;

namespace LogikaGeneracji
{
   public class MapaGeograficzna : IZbiorPunktowGeograficznych
   {
      public IEnumerable<IPunktGeograficzny> PunktyGeograficzne { get; set; }
   }

   public interface IZbiorPunktowGeograficznych
   {
      IEnumerable<IPunktGeograficzny> PunktyGeograficzne { get; set; } 
   }
}