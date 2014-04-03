using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji.PrzetwarzaczeMapy.Baza
{
   public class KreatorCiaguPrzetwarzan
   {
      public static void UstawNastepstwa(IList<IPrzetwarzaczMapy> przetwarzacze)
      {
         przetwarzacze.Aggregate((poprzedni, kolejny) => poprzedni.Nastepnik = kolejny);
      }
   }
}