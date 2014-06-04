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

      public const int PoczRozmiarX = 18;
      public const int PoczRozmiarZ = 18;
      public const int MinRozmiar = 8;
      public const int MaksRozmiar = 75;

      public const float PoczRozpietosc = 2.5f;
      public const float MinRozpietosc = 1f;
      public const float MaksRozpietosc = 5f;
      public const float PoczPoziomMorza = 0f;
      public const float MinPoziomMorza = 0f;
      public const float MaksPoziomMorza = 5f;

      public const float PoczStopienZaburzeniaWezlow = .4f; // 0�1
   }
}