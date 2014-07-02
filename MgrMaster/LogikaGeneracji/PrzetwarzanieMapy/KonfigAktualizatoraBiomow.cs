using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class KonfigAktualizatoraBiomow
   {
      private List<KonfiguracjaBiomu> KonfiguracjeBiomow { get; set; }

      public KonfigAktualizatoraBiomow(List<KonfiguracjaBiomu> dane)
      {
         KonfiguracjeBiomow = dane;
      }

      public Biom PobierzBiom(float temp, float wilg)
      {
         return KonfiguracjeBiomow.Aggregate(
            (poprzednia, nastepna) => 
               (poprzednia == null || poprzednia.OdlegloscDo2(wilg, temp) > nastepna.OdlegloscDo2(wilg, temp) 
                       ? nastepna : poprzednia)).Biom;
      }

   }

   public class KonfiguracjaBiomu
   {
      public Biom Biom { get; set; }
      public float Wilgotnosc { get; set; }
      public float Temperatura { get; set; }

      public KonfiguracjaBiomu(float wilg, float temp, Biom biom)
      {
         Wilgotnosc = wilg;
         Temperatura = temp;
         Biom = biom;
      }

      public float OdlegloscDo2(float wilg, float temp)
      {
         return (Wilgotnosc - wilg)*(Wilgotnosc - wilg) + (Temperatura - temp)*(Temperatura - temp);
      }
   }

}

