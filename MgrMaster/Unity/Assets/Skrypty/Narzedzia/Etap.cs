 using System;

namespace Assets.Skrypty.Narzedzia
{
   [Serializable]
   public enum Etap
   {
      GenerowanieWezlow = 0,
      ZaburzanieWezlow = 1,
      TworzenieKomorekIRogow = 2,
      TworzenieMapyWysokosci = 3,
      RozdzielanieZiemiIWody = 4,
      WydzielanieMorza = 5,
      TworzenieJezior = 6,
      TworzenieRzek = 7,
      WyznaczanieWilgotnosci = 8,
      WyznaczanieTemperatury = 9,
      WyznaczanieBiomow = 10,
      Koniec = 11
   }
}