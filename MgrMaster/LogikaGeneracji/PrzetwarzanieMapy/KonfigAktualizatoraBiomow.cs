using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class KonfigAktualizatoraBiomow
   {
      /// <summary>
      /// Pierwszy indeks to temperatura, drugi — wilgotność
      /// </summary>
      private Biom[,] TabelaBiomow { get; set; }

      public float _normalizacjaWilgotnosci;
      public float _normalizacjaTemperatury;

      public KonfigAktualizatoraBiomow(float normalizacjaWilgotnosci, float normalizacjaTemperatury, Biom[,] tabelaBiomów = null)
      {
         _normalizacjaWilgotnosci = normalizacjaWilgotnosci;
         _normalizacjaTemperatury = normalizacjaTemperatury;
         TabelaBiomow = tabelaBiomów;
      }

      public Biom PobierzBiom(float temperatura, float wilgotnosc)
      {
         var indeksTemperatury = (int)(temperatura / _normalizacjaTemperatury);
         indeksTemperatury = Mathf.Clamp(indeksTemperatury, 0, TabelaBiomow.GetLength(0)-1);

         var indeksWilgotnosci = (int)(wilgotnosc / _normalizacjaWilgotnosci);
         indeksWilgotnosci = Mathf.Clamp(indeksWilgotnosci, 0, TabelaBiomow.GetLength(1)-1);

         //Debug.Log(TabelaBiomow.GetLength(0));
         //Debug.Log(TabelaBiomow.GetLength(1));
         //Debug.Log(indeksTemperatury);
         //Debug.Log(indeksWilgotnosci);

         return TabelaBiomow[indeksTemperatury, indeksWilgotnosci];
      }

   }

}

