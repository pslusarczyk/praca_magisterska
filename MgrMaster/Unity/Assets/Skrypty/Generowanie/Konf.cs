using UnityEngine;

namespace Assets.Skrypty.Generowanie
{
   public static class Konf
   {
      public static GUIStyle StylNaglowkaInspektora = new GUIStyle
      {
         fontSize = 18,
         alignment = TextAnchor.MiddleCenter,
      };

      public static GUIStyle StylWazny = new GUIStyle
      {
         fontSize = 12,
         alignment = TextAnchor.MiddleCenter,
         normal = new GUIStyleState
         {
            textColor = Color.red
         }
      };

      public const int PoczRozmiarX = 40;
      public const int PoczRozmiarZ = 40;
      public const int MinRozmiar = 8;
      public const int MaksRozmiar = 100;

      public const float PoczRozpietosc = 1.4f;
      public const float MinRozpietosc = 1f;
      public const float MaksRozpietosc = 3f;
      public const float PoczPoziomMorza = 2.9f;
      public const float MinPoziomMorza = 2f;
      public const float MaksPoziomMorza = 5f;

      public const float PoczStopienZaburzeniaWezlow = .4f; // 0�1

      public static class Perlin
      {
         public const int MinIloscWarstw = 1;
         public const int MaksIloscWarstw = 4;
         public const int PoczIloscWarstw = 3;

         public const float MinSkala = 0.5f;
         public const float MaksSkala = 8f;
         public const float PoczSkala = 4.4f;

         public const float MinStrataSkali = 0.2f;
         public const float MaksStrataSkali = 0.9f;
         public const float PoczStrataSkali = 0.4f;

         public const float MinSkokGestosci = 1f;
         public const float MaksSkokGestosci = 5f;
         public const float PoczSkokGestosci = 3f;

         public const float MinGestosc = 0.01f;
         public const float MaksGestosc = 0.2f;
         public const float PoczGestosc = 0.05f;
      }
   }
}