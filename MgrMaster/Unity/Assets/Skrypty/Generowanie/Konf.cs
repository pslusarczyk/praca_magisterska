using System.Collections.Generic;
using LogikaGeneracji;
using LogikaGeneracji.PrzetwarzanieMapy;
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

      public const int MaksLiczbaRzekDoWygenerowaniaNaraz = 10;
      public const float PoczMnoznikTemperatury = 1f;
      public const float MnoznikTemperaturyMin = 0f;
      public const float MnoznikTemperaturyMaks = 2f;

      public const int LimitProbUtworzeniaSrednioJednejRzeki = 10;
      public const float MinimalnaWysokoscZrodlaRzeki = .4f;

      public static class Perlin
      {
         public const int MinIloscWarstw = 1;
         public const int MaksIloscWarstw = 4;
         public const int PoczIloscWarstw = 3;

         public const float MinSkala = 0.5f;
         public const float MaksSkala = 8f;
         public const float PoczSkala = 4.4f;

         public const float MinZachowanieSkali = 0.2f;
         public const float MaksZachowanieSkali = 0.9f;
         public const float PoczZachowanieSkali = 0.4f;

         public const float MinSkokGestosci = 1f;
         public const float MaksSkokGestosci = 5f;
         public const float PoczSkokGestosci = 3f;

         public const float MinGestosc = 0.01f;
         public const float MaksGestosc = 0.2f;
         public const float PoczGestosc = 0.05f;
      }

      public static class Wilg
      {
         public const int PoczGlebokoscPrzeszukiwania = 10;
         public const int MinGlebokoscPrzeszukiwania = 4;
         public const int MaksGlebokoscPrzeszukiwania = 15;

         public const float PoczWartoscJeziora = 3f;
         public const float PoczWartoscRzeki = 2f;
         public const float PoczWartoscMorza = 1f;

         public const float MinWartoscJezioraRzekiMorza = 0f;
         public const float MaksWartoscJezioraRzekiMorza = 5f;

         public const float MnoznikWartosci = .5f;
      }

      public static readonly KonfigAktualizatoraBiomow KonfiguracjaBiomow = new KonfigAktualizatoraBiomow(
         new List<KonfiguracjaBiomu>
         {
            new KonfiguracjaBiomu(0.4f, 0.13f, Biom.WiecznySnieg),
            new KonfiguracjaBiomu(0.3f, 0.18f, Biom.GoleGory),
            new KonfiguracjaBiomu(0.3f, 0.22f, Biom.Kosodrzewina),
            new KonfiguracjaBiomu(0.4f, 0.24f, Biom.Tajga),
            new KonfiguracjaBiomu(0.3f, 0.29f, Biom.LasUmiarkowany),
            new KonfiguracjaBiomu(0.6f, 0.25f, Biom.LasUmiarkowany),
            new KonfiguracjaBiomu(0.4f, 0.6f, Biom.LasUmiarkowany),
            new KonfiguracjaBiomu(0.35f, 0.7f, Biom.Laki),
            new KonfiguracjaBiomu(0.12f, 0.9f, Biom.Pustynia),
            new KonfiguracjaBiomu(0.9f, 0.5f, Biom.Bagna),
            new KonfiguracjaBiomu(0.8f, 0.8f, Biom.LasWilgotny),
         }
         );

      public static readonly Dictionary<Biom, Color> KolorBiomu = new Dictionary<Biom, Color>
      {
         {Biom.WiecznySnieg, new Color(1f, 1f, 1f)},
         {Biom.GoleGory, new Color(.3f, .3f, .3f)},
         {Biom.Kosodrzewina, new Color(.35f, .8f, .5f)},
         {Biom.Tajga, new Color(.2f, .37f, .25f)},
         {Biom.LasWilgotny, new Color(.25f, .3f, .12f)},
         {Biom.Laki, new Color(.6f, .9f, .18f)},
         {Biom.Pustynia, new Color(.9f, 1f, .24f)},
         {Biom.Bagna, new Color(.43f, .44f, .20f)},
         {Biom.LasUmiarkowany, new Color(.23f, .7f, .02f)},
      };
   }
}