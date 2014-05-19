using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;
using Random = System.Random;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorWysokosciPerlinem : BazaPrzetwarzacza
   {
      readonly Random Rand = new Random();

      public override void Przetwarzaj(IMapa mapa)
      {
         const float skalaWysokosci = 4f;
         var przesuniecieX = (float)Rand.NextDouble();
         var przesuniecieZ = (float)Rand.NextDouble();
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = Mathf.PerlinNoise(punkt.Pozycja.x*.08f + przesuniecieX, 
                                               punkt.Pozycja.z*.08f + przesuniecieZ)
                                               * skalaWysokosci;
         }
      }
   }
}