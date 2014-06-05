using System.Runtime.InteropServices;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;
using Random = System.Random;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorWysokosciPerlinem : BazaPrzetwarzacza
   {
      private Random _rand;
      private readonly ParametryPerlina _parametryPerlina;

      public ModyfikatorWysokosciPerlinem(ParametryPerlina parametryPerlina)
      {
         _parametryPerlina = parametryPerlina;
      }

      public override void Przetwarzaj(IMapa mapa)
      {
         _rand = new Random(_parametryPerlina.Ziarno);
         var przesuniecieX = (float)_rand.NextDouble() * 4096f;
         var przesuniecieZ = (float)_rand.NextDouble() * 4096f;
         float wspolczynnik = 1f;
         for (int warstwa = 2; warstwa <= _parametryPerlina.IloscWarstw; ++warstwa)
         {
            wspolczynnik += Mathf.Pow(_parametryPerlina.StrataSkali, warstwa);
            }
         float znormalizowanaSkalaPoczatkowa = _parametryPerlina.SkalaPoczatkowa/wspolczynnik;

         foreach (IPunkt punkt in mapa.Punkty)
         {
            float wysokosc = 0;
            for (int warstwa = 0; warstwa < _parametryPerlina.IloscWarstw; ++warstwa)
            {
               float gestosc = _parametryPerlina.GestoscPoczatkowa * Mathf.Pow(_parametryPerlina.SkokGestosci, warstwa); // rosnie
               float skala = znormalizowanaSkalaPoczatkowa * Mathf.Pow(_parametryPerlina.StrataSkali, warstwa); // maleje
               wysokosc += Mathf.PerlinNoise((punkt.Pozycja.x + przesuniecieX) * gestosc,
                                             (punkt.Pozycja.z + przesuniecieZ) * gestosc)
                                                      *skala;
            }
            punkt.Wysokosc = wysokosc;
         }
      }
   }
}