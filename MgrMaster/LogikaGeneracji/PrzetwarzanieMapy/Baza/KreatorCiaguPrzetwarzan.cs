using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji.PrzetwarzanieMapy.Baza
{
   public class KreatorCiaguPrzetwarzan
   {
      public static void UstawNastepstwa(IList<IPrzetwarzaczMapy> przetwarzacze)
      {
         przetwarzacze.Aggregate((poprzedni, kolejny) => poprzedni.Nastepnik = kolejny);
      }
   }
}