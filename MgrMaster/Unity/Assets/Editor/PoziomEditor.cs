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
      private int _ziarnoGenerowaniaRzek;

      [ExposeProperty]
      public string EtapTekst { get { return _stanGeneratora.Etap.ToString(); } set {} } // musi byæ set ¿eby siê wyœwietla³o

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
         if (_stanGeneratora.Etap == Etap.TworzenieMapyWysokosci || _stanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
            SekcjaGenerowaniaMapyWysokosci();
         if (_stanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
            SekcjaPoziomuMorza();
         if (_stanGeneratora.Etap == Etap.WydzielanieMorza)
            SekcjaWydzielaniaMorza();
         if (_stanGeneratora.Etap == Etap.TworzenieJezior)
            SekcjaTworzeniaJezior();
         if (_stanGeneratora.Etap == Etap.TworzenieRzek)
            SekcjaTworzeniaRzek();
         if (_stanGeneratora.Etap == Etap.WyznaczanieWilgotnosci)
            SekcjaWyznaczaniaWilgotnosci();
         if (_stanGeneratora.Etap == Etap.WyznaczanieTemperatury)
            SekcjaWyznaczaniaTemperatury();
         if (_stanGeneratora.Etap == Etap.WyznaczanieBiomow)
            SekcjaWyznaczaniaBiomow();
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
         EditorGUILayout.LabelField("Okreœl wymiary poziomu:", Konf.StylNaglowkaInspektora);
         StanGeneratora._rozmiarX = EditorGUILayout.IntSlider("Rozmiar X", StanGeneratora._rozmiarX, Konf.MinRozmiar,
            Konf.MaksRozmiar);
         StanGeneratora._rozmiarZ = EditorGUILayout.IntSlider("Rozmiar Z", StanGeneratora._rozmiarZ, Konf.MinRozmiar,
            Konf.MaksRozmiar);
         StanGeneratora._rozpietosc = EditorGUILayout.Slider("Rozpiêtoœæ", StanGeneratora._rozpietosc, Konf.MinRozpietosc,
            Konf.MaksRozpietosc);
         if (GUILayout.Button("Generuj wêz³y"))
         {
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _dzialaniaNaWezlach.GenerujWezly();
            _stanGeneratora.Etap = Etap.ZaburzanieWezlow;
         }
      }

      private void SekcjaZaburzaniaITworzeniaKomorekIRogow()
      {
         EditorGUILayout.LabelField("Okreœl stopieñ zaburzenia:", Konf.StylNaglowkaInspektora);
         StanGeneratora.ZasiegZaburzenia = EditorGUILayout.Slider("Stopieñ zaburzenia", StanGeneratora.ZasiegZaburzenia, 0f,
            1f);
         if (GUILayout.Button("Zaburz wêz³y"))
         {
            _dzialaniaNaWezlach.ZaburzWezly(StanGeneratora.ZasiegZaburzenia*StanGeneratora._rozpietosc, true);
            _stanGeneratora.Etap = Etap.TworzenieKomorekIRogow;
         }
         if (GUILayout.Button("Utwórz komórki i rogi"))
         {
            _dzialaniaNaWezlach.UkryjWezly();
            _dzialaniaNaWezlach.GenerujKomorkiIRogi();
            _stanGeneratora.Etap = Etap.TworzenieMapyWysokosci;
         }
      }

      private void SekcjaPokazywaniaIUkrywaniaScianIrogow()
      {
         PokazSciany = GUILayout.Toggle(PokazSciany, "Poka¿ œciany"); // todo czemu to siê nie zapisuje do stanu generatora?
         StanGeneratora.PokazRogi = GUILayout.Toggle(StanGeneratora.PokazRogi, "Poka¿ rogi");
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
         if (GUILayout.Button("Generuj wysokoœci"))
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
         EditorGUILayout.LabelField("Okreœl poziom morza", Konf.StylNaglowkaInspektora);
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
            _stanGeneratora.Etap = Etap.WydzielanieMorza;
            _dzialaniaNaMapie.UstawKomorkomWidocznoscPolaInicjatorPowodzi(true);
         }
      }

      private void SekcjaWydzielaniaMorza()
      {
         EditorGUILayout.LabelField("Wybierz komórki inicjuj¹ce powódŸ", Konf.StylNaglowkaInspektora);
         _stanGeneratora.InicjatorzyZalewania = Poziom._komorkiUnity.Where(k => k.InicjatorPowodzi);
         if (GUILayout.Button("Rozdziel morze i jeziora"))
         {
            _dzialaniaNaMapie.RozdzielMorzeIJeziora(_stanGeneratora.InicjatorzyZalewania);
            _dzialaniaNaMapie.AktualizujBrzeznosci();
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
         }
         if (GUILayout.Button("Dalej"))
         {
            _dzialaniaNaMapie.UstawPunktomNastepstwaMapyWysokosci();
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
         if (GUILayout.Button("Utwórz jeziora"))
         {
            _dzialaniaNaMapie.UtworzJezioraWNieckach();
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         }
         if (GUILayout.Button("Dalej"))
         {
            _stanGeneratora.Etap = Etap.TworzenieRzek;
         }
      }

      private void SekcjaTworzeniaRzek()
      {
         if (GUILayout.Button("Utwórz rzeki"))
         {
            Debug.Log("Ziarno generowania rzek = " + _ziarnoGenerowaniaRzek);
            _dzialaniaNaMapie.UtworzRzeki(new System.Random(_ziarnoGenerowaniaRzek++));
            _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            OdswiezZaznaczenieWarstwy();
         }
         if (GUILayout.Button("Dalej"))
         {
            _stanGeneratora.Etap = Etap.WyznaczanieWilgotnosci;
         }
      }

      private void SekcjaWyznaczaniaWilgotnosci()
      {
         PokazPanelWarstw();

         StanGeneratora.ParametryWilgotnosci.GlebokoscPrzeszukiwania = EditorGUILayout.IntSlider("G³êbokoœæ przeszukiwania",
      StanGeneratora.ParametryWilgotnosci.GlebokoscPrzeszukiwania, Konf.Wilg.MinGlebokoscPrzeszukiwania, Konf.Wilg.MaksGlebokoscPrzeszukiwania);
         StanGeneratora.ParametryWilgotnosci.WartoscJeziora = EditorGUILayout.Slider("Wilgotnoœæ jeziora",
      StanGeneratora.ParametryWilgotnosci.WartoscJeziora, Konf.Wilg.MinWartoscJezioraRzekiMorza, Konf.Wilg.MaksWartoscJezioraRzekiMorza);
         StanGeneratora.ParametryWilgotnosci.WartoscMorza = EditorGUILayout.Slider("Wilgotnoœæ morza",
      StanGeneratora.ParametryWilgotnosci.WartoscMorza, Konf.Wilg.MinWartoscJezioraRzekiMorza, Konf.Wilg.MaksWartoscJezioraRzekiMorza);
         StanGeneratora.ParametryWilgotnosci.WartoscRzeki = EditorGUILayout.Slider("Wilgotnoœæ rzeki",
      StanGeneratora.ParametryWilgotnosci.WartoscRzeki, Konf.Wilg.MinWartoscJezioraRzekiMorza, Konf.Wilg.MaksWartoscJezioraRzekiMorza);
         
         if (GUILayout.Button("Utwórz mapê wilgotnoœci"))
         {
            _dzialaniaNaMapie.UtworzMapeWilgotnosci(StanGeneratora.ParametryWilgotnosci);

            if (!StanGeneratora._utworzoneWarstwy.Contains(Warstwa.Wilgotnosc))
               StanGeneratora._utworzoneWarstwy.Add(Warstwa.Wilgotnosc);
            AktualnaWarstwa = Warstwa.Wilgotnosc;
            _stanGeneratora.NumerWybranejWarstwy = StanGeneratora._utworzoneWarstwy.IndexOf(AktualnaWarstwa);
            _dzialaniaNaMapie.PokazWarstweWilgotnosci();
            StanGeneratora.Etap = Etap.WyznaczanieTemperatury;
         }
      }

      private void SekcjaWyznaczaniaTemperatury()
      {
         PokazPanelWarstw();

         if (GUILayout.Button("Utwórz mapê temperatury"))
         {
            _dzialaniaNaMapie.UtworzMapeTemperatury();

            if (!StanGeneratora._utworzoneWarstwy.Contains(Warstwa.Temperatura))
               StanGeneratora._utworzoneWarstwy.Add(Warstwa.Temperatura);
            AktualnaWarstwa = Warstwa.Temperatura;
            _stanGeneratora.NumerWybranejWarstwy = StanGeneratora._utworzoneWarstwy.IndexOf(AktualnaWarstwa);
            _dzialaniaNaMapie.PokazWarstweTemperatury();
            StanGeneratora.Etap = Etap.WyznaczanieBiomow;
         }
      }

      private void SekcjaWyznaczaniaBiomow()
      {
         PokazPanelWarstw();

         StanGeneratora.NormTemp = EditorGUILayout.Slider("Norm temp", StanGeneratora.NormTemp, 0f, 5f);
         StanGeneratora.NormWilg = EditorGUILayout.Slider("Norm wilg", StanGeneratora.NormWilg, 0f, 5f);

         if (GUILayout.Button("Wyznacz biomy"))
         {
            _dzialaniaNaMapie.UtworzMapeBiomow();

            if (!StanGeneratora._utworzoneWarstwy.Contains(Warstwa.Biomy))
               StanGeneratora._utworzoneWarstwy.Add(Warstwa.Biomy);
            AktualnaWarstwa = Warstwa.Biomy;
            _stanGeneratora.NumerWybranejWarstwy = StanGeneratora._utworzoneWarstwy.IndexOf(AktualnaWarstwa);
            _dzialaniaNaMapie.PokazWarstweBiomow();
         }
      }

      private void PokazPanelWarstw()
      {
         GUILayout.Label("Wybierz warstwê:");
         _stanGeneratora.NumerWybranejWarstwy = GUILayout.SelectionGrid(_stanGeneratora.NumerWybranejWarstwy,
            StanGeneratora._utworzoneWarstwy.ToList().Select(w => w.ToString()).ToArray(),
            1,GUILayout.Width(200));
         GUILayout.Space(20);

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
            if (Poziom.AktualnaWarstwa == Warstwa.Wilgotnosc)
            {
               _dzialaniaNaMapie.PokazWarstweWilgotnosci();
               OdswiezZaznaczenieWarstwy();
            }
            if (Poziom.AktualnaWarstwa == Warstwa.Temperatura)
            {
               _dzialaniaNaMapie.PokazWarstweTemperatury();
               OdswiezZaznaczenieWarstwy();
            }
            if (Poziom.AktualnaWarstwa == Warstwa.Biomy)
            {
               _dzialaniaNaMapie.PokazWarstweBiomow();
               OdswiezZaznaczenieWarstwy();
            }
         }
      }

      public void OdswiezZaznaczenieWarstwy()
      {
         _stanGeneratora.NumerWybranejWarstwy = StanGeneratora._utworzoneWarstwy.IndexOf(AktualnaWarstwa);
      }
   }
}