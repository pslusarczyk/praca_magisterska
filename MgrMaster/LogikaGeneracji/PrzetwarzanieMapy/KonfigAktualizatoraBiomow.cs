using System.Collections.Generic;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class KonfigAktualizatoraBiomow
   {
      /// <summary>
      /// Pierwszy indeks to temperatura, drugi — wilgotność
      /// </summary>
      private Biom[,] TabelaBiomow { get; set; }

      private readonly float _wspolczynnikNormalizacji;

      public KonfigAktualizatoraBiomow(float wspolczynnikNormalizacji, Biom[,] tabelaBiomów = null)
      {
         _wspolczynnikNormalizacji = wspolczynnikNormalizacji;
         TabelaBiomow = tabelaBiomów;
      }

      public Biom PobierzBiom(float temperatura, float wilgotnosc)
      {
         var indeksTemperatury = (int) (temperatura/_wspolczynnikNormalizacji);
         var indeksWilgotnosci = (int) (wilgotnosc/_wspolczynnikNormalizacji);
         return TabelaBiomow[indeksTemperatury, indeksWilgotnosci];
      }

   }

}
