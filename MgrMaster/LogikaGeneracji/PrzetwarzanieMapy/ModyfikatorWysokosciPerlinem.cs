using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;
using Random = System.Random;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorWysokosciPerlinem : BazaPrzetwarzacza
   {
      readonly Random Rand = new Random();
      private ParametryPerlina _parametryPerlina;

      public ModyfikatorWysokosciPerlinem(ParametryPerlina parametryPerlina)
      {
         _parametryPerlina = parametryPerlina;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         
         
         
         
         
         var przesuniecieX = (float)Rand.NextDouble() * 4096f;
         var przesuniecieZ = (float)Rand.NextDouble() * 4096f;
         foreach (IPunkt punkt in mapa.Punkty)
         {
            float wysokosc = 0;
            for (int warstwa = 0; warstwa < _parametryPerlina.IloscWarstw; ++warstwa)
            {
               float gestosc = _parametryPerlina.GestoscPoczatkowa * Mathf.Pow(_parametryPerlina.SkokGestosci, warstwa); // rosnie
               float skala = _parametryPerlina.SkalaPoczatkowa * Mathf.Pow(_parametryPerlina.StrataSkali, warstwa); // maleje
               wysokosc += Mathf.PerlinNoise((punkt.Pozycja.x + przesuniecieX) * gestosc,
                                             (punkt.Pozycja.z + przesuniecieZ) * gestosc)
                                                      *skala;
            }
            punkt.Wysokosc = wysokosc;
         }
      }
   }
}