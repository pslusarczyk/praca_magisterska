using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji
{
   public interface IZbiorPunktow
   {
      IEnumerable<IPunkt> Punkty { get; }
   }

   public class MapaProsta : IZbiorPunktow
   {
      public bool ZakonczonoTworzenie { get; set; }
      public List<Dwukrawedz> Dwukrawedzie { get; set; }
      public HashSet<IKomorka> Komorki { get; set; }
      public HashSet<IRog> Rogi { get; set; }

      public MapaProsta()
      {
         ZakonczonoTworzenie = false;
      }

      public IEnumerable<IPunkt> Punkty
      {
         get
         {
            return new List<IPunkt>(
               Rogi.Select(r => r as IPunkt))
               .Union(Komorki.Select(k => k as IPunkt)).ToList();
         }
      }
   }
}