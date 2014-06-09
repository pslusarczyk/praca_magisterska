using System;
using System.Linq;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using Assets.Skrypty.Narzedzia;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
   [CustomEditor(typeof(Poziom))]
   public class PoziomEditor : UnityEditor.Editor
   {
      PropertyField[] m_fields;
      private Poziom _poziom;
      private Warstwa _poprzedniaWarstwa;
      
      public bool PokazSciany
      {
         get { return Poziom._pokazSciany; }
         set
         {  SceneView.RepaintAll(); Poziom._pokazSciany = value; }
      }

      public Warstwa AktualnaWarstwa
      {
         get { return _poziom.AktualnaWarstwa; }
         set { _poziom.AktualnaWarstwa = value; }
      }

      private readonly DzialaniaNaMapie _dzialaniaNaMapie;

      private readonly DzialaniaNaWezlach _dzialaniaNaWezlach;

      private StanGeneratora _stanGeneratora;

      private float _poprzedniPoziomMorza = Konf.PoczPoziomMorza;

      [ExposeProperty]
      public string EtapTekst { get { return _stanGeneratora.Etap.ToString(); } set {} } // musi by� set �eby si� wy�wietla�o

      public Poziom Poziom
      {
         get { return _poziom ?? (_poziom = (Poziom)target); }
         set { _poziom = value; }
      }

      public Warstwa PoprzedniaWarstwa
      {
         set { _poprzedniaWarstwa = value; }
         get { return _poprzedniaWarstwa; }
      }

      public StanGeneratora StanGeneratora
      {
         get { return _stanGeneratora; }
      }

      public PoziomEditor()
      {
         _dzialaniaNaMapie = new DzialaniaNaMapie(this);
         _dzialaniaNaWezlach = new DzialaniaNaWezlach(this);
      }

      public void OnEnable()
      {
         _poziom = target as Poziom;
         _stanGeneratora = _poziom.StanGeneratora ?? _stanGeneratora;
         _poprzedniaWarstwa = Warstwa.Brak;
         m_fields = ExposeProperties.ExposeProperties.GetProperties(_poziom);
      }

      public override void OnInspectorGUI()
      {
         if (_poziom == null)
            return;
         DrawDefaultInspector();
         ExposeProperties.ExposeProperties.Expose(m_fields);

         EditorGUILayout.LabelField("Generator poziomu", Konf.StylNaglowkaInspektora);

         SekcjaResetowania();
         if (_stanGeneratora.Etap == Etap.GenerowanieWezlow)
            SekcjaGenerowaniaWezlow();
         if ((_stanGeneratora.Etap == Etap.ZaburzanieWezlow || _stanGeneratora.Etap == Etap.TworzenieKomorekIRogow))
            SekcjaZaburzaniaITworzeniaKomorekIRogow();
         if (_stanGeneratora.Etap >= Etap.TworzenieMapyWysokosci)
            SekcjaPokazywaniaIUkrywaniaScianIrogow();
         if (_stanGeneratora.Etap >= Etap.TworzenieMapyWysokosci || _stanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
            SekcjaGenerowaniaMapyWysokosci();
         if (_stanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
            SekcjaPoziomuMorza();
         if (_stanGeneratora.Etap == Etap.WydzielanieMorza)
            SekcjaWydzielaniaMorza();
         if (_stanGeneratora.Etap == Etap.TworzenieJezior)
            SekcjaTworzeniaJezior();

      }

      private void SekcjaResetowania()
      {
         GUI.color = new Color(.9f, .1f, .1f);
         if (GUILayout.Button("Resetuj"))
         {
            StanGeneratora.PokazRogi = false;
            StanGeneratora.PokazRogiPoprzedniaWartosc = true;
            PokazSciany = false;
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _stanGeneratora.Etap = Etap.GenerowanieWezlow;
            StanGeneratora._utworzoneWarstwy.Clear();
         }
         GUI.color = Color.white;
      }

      private void SekcjaGenerowaniaWezlow()
      {
         EditorGUILayout.LabelField("Okre�l wymiary poziomu:", Konf.StylNaglowkaInspektora);
         StanGeneratora._rozmiarX = EditorGUILayout.IntSlider("Rozmiar X", StanGeneratora._rozmiarX, Konf.MinRozmiar,
            Konf.MaksRozmiar);
         StanGeneratora._rozmiarZ = EditorGUILayout.IntSlider("Rozmiar Z", StanGeneratora._rozmiarZ, Konf.MinRozmiar,
            Konf.MaksRozmiar);
         StanGeneratora._rozpietosc = EditorGUILayout.Slider("Rozpi�to��", StanGeneratora._rozpietosc, Konf.MinRozpietosc,
            Konf.MaksRozpietosc);
         if (GUILayout.Button("Generuj w�z�y"))
         {
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _dzialaniaNaWezlach.GenerujWezly();
            _stanGeneratora.Etap = Etap.ZaburzanieWezlow;
         }
      }

      private void SekcjaZaburzaniaITworzeniaKomorekIRogow()
      {
         EditorGUILayout.LabelField("Okre�l stopie� zaburzenia:", Konf.StylNaglowkaInspektora);
         StanGeneratora.ZasiegZaburzenia = EditorGUILayout.Slider("Stopie� zaburzenia", StanGeneratora.ZasiegZaburzenia, 0f,
            1f);
         if (GUILayout.Button("Zaburz w�z�y"))
         {
            _dzialaniaNaWezlach.ZaburzWezly(StanGeneratora.ZasiegZaburzenia*StanGeneratora._rozpietosc, true);
            _stanGeneratora.Etap = Etap.TworzenieKomorekIRogow;
         }
         if (GUILayout.Button("Utw�rz kom�rki i rogi"))
         {
            _dzialaniaNaWezlach.UkryjWezly();
            _dzialaniaNaWezlach.GenerujKomorkiIRogi();
            _stanGeneratora.Etap = Etap.TworzenieMapyWysokosci;
         }
      }

      private void SekcjaPokazywaniaIUkrywaniaScianIrogow()
      {
         PokazSciany = GUILayout.Toggle(PokazSciany, "Poka� �ciany"); // todo czemu to si� nie zapisuje do stanu generatora?
         StanGeneratora.PokazRogi = GUILayout.Toggle(StanGeneratora.PokazRogi, "Poka� rogi");
         if (StanGeneratora.PokazRogi != StanGeneratora.PokazRogiPoprzedniaWartosc)
         {
            StanGeneratora.PokazRogiPoprzedniaWartosc = StanGeneratora.PokazRogi;
            if (!StanGeneratora.PokazRogi)
               _dzialaniaNaMapie.UkryjRogi();
            else
               _dzialaniaNaMapie.PokazRogi();
         }
      }

      private void SekcjaGenerowaniaMapyWysokosci()
      {
         GUILayout.BeginVertical(new GUIStyle
         {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(100, 100, 0, 0)
         });
         if (false && StanGeneratora._utworzoneWarstwy.Count > 0) // na razie nie pokazujemy
         {
            GUILayout.Label("Wybierz warstw�:");
            _stanGeneratora.NumerWybranejWarstwy = GUILayout.SelectionGrid(_stanGeneratora.NumerWybranejWarstwy,
               StanGeneratora._utworzoneWarstwy.ToList().Select(w => w.ToString()).ToArray(),
               1);

            AktualnaWarstwa = StanGeneratora._utworzoneWarstwy[_stanGeneratora.NumerWybranejWarstwy];
            OdswiezZaznaczenieWarstwy();

            if (Poziom.AktualnaWarstwa != PoprzedniaWarstwa)
            {
               PoprzedniaWarstwa = Poziom.AktualnaWarstwa;
               if (Poziom.AktualnaWarstwa == Warstwa.WysokosciZWoda)
               {
                  _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
                  OdswiezZaznaczenieWarstwy();
               }
            }
         }
         GUILayout.EndVertical();

         StanGeneratora.ParametryPerlina.Ziarno
            = EditorGUILayout.IntField("Ziarno", StanGeneratora.ParametryPerlina.Ziarno);

         StanGeneratora.ParametryPerlina.IloscWarstw
            = EditorGUILayout.IntSlider("IloscWarstw",
               StanGeneratora.ParametryPerlina.IloscWarstw, Konf.Perlin.MinIloscWarstw, Konf.Perlin.MaksIloscWarstw);

         StanGeneratora.ParametryPerlina.Skala
            = EditorGUILayout.Slider("Skala",
               StanGeneratora.ParametryPerlina.Skala, Konf.Perlin.MinSkala, Konf.Perlin.MaksSkala);

         StanGeneratora.ParametryPerlina.StrataSkali
            = EditorGUILayout.Slider("StrataSkali",
               StanGeneratora.ParametryPerlina.StrataSkali, Konf.Perlin.MinStrataSkali, Konf.Perlin.MaksStrataSkali);

         StanGeneratora.ParametryPerlina.SkokGestosci
            = EditorGUILayout.Slider("SkokGestosci",
               StanGeneratora.ParametryPerlina.SkokGestosci, Konf.Perlin.MinSkokGestosci, Konf.Perlin.MaksSkokGestosci);

         StanGeneratora.ParametryPerlina.Gestosc
            = EditorGUILayout.Slider("Gestosc",
               StanGeneratora.ParametryPerlina.Gestosc, Konf.Perlin.MinGestosc, Konf.Perlin.MaksGestosc);
         if (GUILayout.Button("Generuj wysoko�ci"))
         {
            _dzialaniaNaMapie.GenerujWysokosci(StanGeneratora.ParametryPerlina);
            if (!StanGeneratora._utworzoneWarstwy.Contains(Warstwa.WysokosciZWoda))
               StanGeneratora._utworzoneWarstwy.Add(Warstwa.WysokosciZWoda);
            AktualnaWarstwa = Warstwa.WysokosciZWoda;

            _dzialaniaNaMapie.RozdzielZiemieIWode(StanGeneratora.PoziomMorza);
            AktualnaWarstwa = Warstwa.WysokosciZWoda;
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
            _stanGeneratora.Etap = Etap.RozdzielanieZiemiIWody;
         }
      }

      private void SekcjaPoziomuMorza()
      {
         EditorGUILayout.LabelField("Okre�l poziom morza", Konf.StylNaglowkaInspektora);
         StanGeneratora.PoziomMorza = EditorGUILayout.Slider("Poziom morza", StanGeneratora.PoziomMorza, Konf.MinPoziomMorza,
            Konf.MaksPoziomMorza);
         if (StanGeneratora.PoziomMorza != _poprzedniPoziomMorza)
         {
            _poprzedniPoziomMorza = StanGeneratora.PoziomMorza;
            _dzialaniaNaMapie.RozdzielZiemieIWode(StanGeneratora.PoziomMorza);
            AktualnaWarstwa = Warstwa.WysokosciZWoda;
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         }
         if (GUILayout.Button("Dalej"))
         {
            _dzialaniaNaMapie.ZatwierdzRozdzielenieZiemiIWody(StanGeneratora.PoziomMorza);
            _dzialaniaNaMapie.UstawPunktomNastepstwaMapyWysokosci();
            _stanGeneratora.Etap = Etap.WydzielanieMorza;
            _dzialaniaNaMapie.UstawKomorkomWidocznoscPolaInicjatorPowodzi(true);
         }
      }

      private void SekcjaWydzielaniaMorza()
      {
         EditorGUILayout.LabelField("Wybierz kom�rki inicjuj�ce pow�d�", Konf.StylNaglowkaInspektora);
         _stanGeneratora.InicjatorzyZalewania = Poziom._komorkiUnity.Where(k => k.InicjatorPowodzi);
         if (GUILayout.Button("Rozdziel morze i jeziora"))
         {
            _dzialaniaNaMapie.RozdzielMorzeIJeziora(_stanGeneratora.InicjatorzyZalewania);
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
         }
         if (GUILayout.Button("Dalej"))
         {
            _dzialaniaNaMapie.UstawKomorkomWidocznoscPolaInicjatorPowodzi(false);
            _stanGeneratora.Etap = Etap.TworzenieJezior;
         }
      }

      private void SekcjaTworzeniaJezior()
      {
         if (GUILayout.Button("Wydziel niecki"))
         {
            _dzialaniaNaMapie.WyznaczKomorkiNiecki();
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         } 
         if (GUILayout.Button("Utw�rz jeziora"))
         {
            _dzialaniaNaMapie.UtworzJezioraWNieckach();
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         }
      }

      public void OdswiezZaznaczenieWarstwy()
      {
         _stanGeneratora.NumerWybranejWarstwy = StanGeneratora._utworzoneWarstwy.IndexOf(AktualnaWarstwa);
      }
   }
}