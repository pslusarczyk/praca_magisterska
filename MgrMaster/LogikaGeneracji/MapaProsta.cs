using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji
{
   public class MapaProsta
   {
      public List<Dwukrawedz> Dwukrawedzie { get; set; }
      public HashSet<IKomorka> Komorki { get; set; }
      public HashSet<IRog> Rogi { get; set; }

      public List<IPunkt> Punkty
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