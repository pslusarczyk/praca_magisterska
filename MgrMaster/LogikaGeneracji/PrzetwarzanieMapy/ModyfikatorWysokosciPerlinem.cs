using LogikaGeneracji.PrzetwarzanieMapy.Baza;
using UnityEngine;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class ModyfikatorWysokosciPerlinem : BazaPrzetwarzacza
   {
      public override void Przetwarzaj(IMapa mapa)
      {
         // float[][] wys = PerlinTools.GeneratePerlinNoise(1000, 1000, 2);
         const float skalaWysokosci = 4f;
         foreach (IPunkt punkt in mapa.Punkty)
         {
            punkt.Wysokosc = Mathf.PerlinNoise(punkt.Pozycja.x*.05f, punkt.Pozycja.z*.05f) * skalaWysokosci;
         }


         //punkt.Wysokosc = 6f + Mathf.Sin(punkt.Pozycja.x * .25f)*3f + Mathf.Cos(punkt.Pozycja.z * .25f)*3f;
         //punkt.Pozycja = new Vector3(punkt.Pozycja.x, punkt.Wysokosc, punkt.Pozycja.z);
      }
   }
}