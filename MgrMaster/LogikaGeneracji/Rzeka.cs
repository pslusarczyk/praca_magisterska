using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji
{
   public interface IRzeka
   {
      IList<IOdcinekRzeki> Odcinki { get; set; }
      int DlugoscDoPunktu(IPunkt prog);
   }

   public class Rzeka : IRzeka
   {
      public IList<IOdcinekRzeki> Odcinki { get; set; }
      public int DlugoscDoPunktu(IPunkt prog)
      {
         return Odcinki.IndexOf(Odcinki.First(o => o.PunktA == prog));
      }
   }

   public interface IOdcinekRzeki
   {
      IPunkt PunktA { get; set; }
      IPunkt PunktB { get; set; }
      float Grubosc { get; set; }
   }

   public class OdcinekRzeki : IOdcinekRzeki
   {
      public IPunkt PunktA { get; set; }
      public IPunkt PunktB { get; set; }
      public float Grubosc { get; set; }
   }
}