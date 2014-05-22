using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Editor.ExposeProperties;
using Assets.Skrypty;
using Assets.Skrypty.Generowanie;
using Assets.Skrypty.Narzedzia;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

         GUI.color = new Color(.9f, .1f, .1f);
         if (GUILayout.Button("Resetuj"))
         {
            StanGeneratora.PokazRogi = false;
            StanGeneratora.PokazRogiPoprzedniaWartosc = true;
            PokazSciany = true;
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _stanGeneratora.Etap = Etap.GenerowanieWezlow;
            StanGeneratora._utworzoneWarstwy.Clear();
         }
         GUI.color = Color.white;

         if (_stanGeneratora.Etap == Etap.GenerowanieWezlow)
         {
            EditorGUILayout.LabelField("Okreœl wymiary poziomu:", Konf.StylNaglowkaInspektora);
            StanGeneratora._rozmiarX = EditorGUILayout.IntSlider("Rozmiar X", StanGeneratora._rozmiarX, Konf.MinRozmiar, Konf.MaksRozmiar);
            StanGeneratora._rozmiarZ = EditorGUILayout.IntSlider("Rozmiar Z", StanGeneratora._rozmiarZ, Konf.MinRozmiar, Konf.MaksRozmiar);
            StanGeneratora._rozpietosc = EditorGUILayout.Slider("Rozpiêtoœæ", StanGeneratora._rozpietosc, Konf.MinRozpietosc, Konf.MaksRozpietosc);
            if (GUILayout.Button("Generuj wêz³y"))
            {
               _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
               _dzialaniaNaWezlach.GenerujWezly();
               _stanGeneratora.Etap = Etap.ZaburzanieWezlow;
            }
         }

         if ((_stanGeneratora.Etap == Etap.ZaburzanieWezlow || _stanGeneratora.Etap == Etap.TworzenieKomorekIRogow))
         {
            EditorGUILayout.LabelField("Okreœl stopieñ zaburzenia:", Konf.StylNaglowkaInspektora);
            StanGeneratora.ZasiegZaburzenia = EditorGUILayout.Slider("Stopieñ zaburzenia", StanGeneratora.ZasiegZaburzenia, 0f, 1f);
            if (GUILayout.Button("Zaburz wêz³y"))
            {
               _dzialaniaNaWezlach.ZaburzWezly(StanGeneratora.ZasiegZaburzenia *StanGeneratora._rozpietosc, true);
               _stanGeneratora.Etap = Etap.TworzenieKomorekIRogow;
            }
            if (GUILayout.Button("Utwórz komórki i rogi"))
            {
               _dzialaniaNaWezlach.UkryjWezly();
               _dzialaniaNaWezlach.GenerujKomorkiIRogi();
               _stanGeneratora.Etap = Etap.TworzenieMapyWysokosci;
            }
         }

         if (_stanGeneratora.Etap >= Etap.TworzenieMapyWysokosci)
         {
            PokazSciany = GUILayout.Toggle(PokazSciany, "Poka¿ œciany");
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

         if (_stanGeneratora.Etap == Etap.TworzenieMapyWysokosci || _stanGeneratora.Etap == Etap.RozdzielanieZiemiIWody)
         {
            GUILayout.BeginVertical(new GUIStyle
            {
               alignment = TextAnchor.MiddleCenter,
               margin = new RectOffset(100, 100, 0, 0)
            });
            if (StanGeneratora._utworzoneWarstwy.Count > 0)
            {
               GUILayout.Label("Wybierz warstwê:");
               _stanGeneratora.NumerWybranejWarstwy = GUILayout.SelectionGrid(_stanGeneratora.NumerWybranejWarstwy, StanGeneratora._utworzoneWarstwy.ToList().Select(w => w.ToString()).ToArray(),
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
            

            if (GUILayout.Button("Generuj wysokoœci"))
            {
               _dzialaniaNaMapie.GenerujWysokosci();
               if (!StanGeneratora._utworzoneWarstwy.Contains(Warstwa.WysokosciZWoda))
                  StanGeneratora._utworzoneWarstwy.Add(Warstwa.WysokosciZWoda);
               AktualnaWarstwa = Warstwa.WysokosciZWoda;
               _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
               OdswiezZaznaczenieWarstwy();
               _stanGeneratora.Etap = Etap.RozdzielanieZiemiIWody;
            } 
         }

         if (_stanGeneratora.Etap >= Etap.RozdzielanieZiemiIWody )
         {
            EditorGUILayout.LabelField("Okreœl poziom morza", Konf.StylNaglowkaInspektora);
            StanGeneratora.PoziomMorza = EditorGUILayout.Slider("Poziom morza", StanGeneratora.PoziomMorza, Konf.MinPoziomMorza, Konf.MaksPoziomMorza);
            if (StanGeneratora.PoziomMorza != StanGeneratora.PoprzedniPoziomMorza)
            {
               StanGeneratora.PoprzedniPoziomMorza = StanGeneratora.PoziomMorza;
               _dzialaniaNaMapie.RozdzielZiemieIWode(StanGeneratora.PoziomMorza);
               if (!StanGeneratora._utworzoneWarstwy.Contains(Warstwa.WysokosciZWoda))
                  StanGeneratora._utworzoneWarstwy.Add(Warstwa.WysokosciZWoda);
               AktualnaWarstwa = Warstwa.WysokosciZWoda;
               _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
               OdswiezZaznaczenieWarstwy();
            }
            if (StanGeneratora.PoziomMorza != Konf.PoczPoziomMorza)
               if (GUILayout.Button("ZatwierdŸ"))
               {
                  _dzialaniaNaMapie.ZatwierdzRozdzielenieZiemiIWody(StanGeneratora.PoziomMorza);
                  _stanGeneratora.Etap = Etap.WydzielanieMorza;
               }
         }

         if (_stanGeneratora.Etap == Etap.WydzielanieMorza)
         {
            EditorGUILayout.LabelField("Wybierz komórkê inicjuj¹c¹ powódŸ", Konf.StylNaglowkaInspektora);
            Object wybranyObiekt = EditorGUILayout.ObjectField(_stanGeneratora.InicjatorZalewania, typeof(GameObject), true);
            if (wybranyObiekt as GameObject)
            {
               _stanGeneratora.InicjatorZalewania = ((GameObject)wybranyObiekt).GetComponent<KomorkaUnity>();
            }
            if (GUILayout.Button("ZatwierdŸ"))
            {
               _dzialaniaNaMapie.RozdzielMorzeIJeziora(_stanGeneratora.InicjatorZalewania);
               _dzialaniaNaMapie.PokazWarstweWysokosciIWody();
            }

         }

      }

      public void OdswiezZaznaczenieWarstwy()
      {
         _stanGeneratora.NumerWybranejWarstwy = StanGeneratora._utworzoneWarstwy.IndexOf(AktualnaWarstwa);
      }
   }
}