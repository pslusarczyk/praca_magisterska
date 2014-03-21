using System.Collections.Generic;

namespace LogikaGeneracji
{
   public class MapaGeograficzna : IZbiorPunktowGeograficznych
   {
      public IEnumerable<IPunktGeograficzny> Punkty { get; set; }
   }

   public interface IZbiorPunktowGeograficznych
   {
      IEnumerable<IPunktGeograficzny> Punkty { get; set; } 
   }
}