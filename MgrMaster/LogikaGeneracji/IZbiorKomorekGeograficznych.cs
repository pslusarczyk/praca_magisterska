using System.Collections.Generic;

namespace LogikaGeneracji
{
   public interface IZbiorKomorekGeograficznych
   {
      IEnumerable<IKomorkaGeograficzna> KomorkiGeograficzne { get; set; }
      void UstawKomorkomSasiedztwa();
   }
}