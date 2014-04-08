using System.Collections.Generic;

namespace LogikaGeneracji
{
   public interface IRzeka
   {
      IList<IPunkt> Punkty { get; set; } 
   }

   public class Rzeka : IRzeka
   {
      public IList<IPunkt> Punkty { get; set; }
   }
}