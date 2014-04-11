using System.Collections.Generic;

namespace LogikaGeneracji
{
   public interface IRzeka
   {
      IList<IMiejsceRzeki> MiejscaRzeki { get; set; } 
   }

   public class Rzeka : IRzeka
   {
      public IList<IMiejsceRzeki> MiejscaRzeki { get; set; }
   }

   public interface IMiejsceRzeki
   {
      IPunkt Punkt { get; set; }
      int DlugoscDotad { get; set; }
      float Grubosc { get; set; }
   }

   public class MiejsceRzeki : IMiejsceRzeki
   {
      public IPunkt Punkt { get; set; }
      public int DlugoscDotad { get; set; }
      public float Grubosc { get; set; }
   }
}