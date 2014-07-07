using System;
using System.Collections.Generic;
using System.Linq;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   [Serializable]
   public class KonfigAktualizatoraBiomow
   {
      public List<KonfiguracjaBiomu> ParametryBiomow { get; set; }

      public KonfigAktualizatoraBiomow(List<KonfiguracjaBiomu> dane)
      {
         ParametryBiomow = dane;
      }

      public Biom PobierzBiom(float temp, float wilg)
      {
         return ParametryBiomow.Aggregate(
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

