using UnityEngine;

namespace Assets.Editor.Konfiguracje
{
   public class Konf
   {
      public static GUIStyle StylNaglowkaInspektora = new GUIStyle
      {
         fontSize = 18,
         alignment = TextAnchor.MiddleCenter,
      };

      public static int PoczRozmiarX = 12;
      public static int PoczRozmiarZ = 12;
      public const int MinRozmiar = 8;
      public const int MaksRozmiar = 75;

      public static float PoczRozpietosc = 2.5f;
      public static float MinRozpietosc = 1f;
      public static float MaksRozpietosc = 5f;

      public const float PoczStopienZaburzeniaWezlow = .5f; // 0–1
   }
}