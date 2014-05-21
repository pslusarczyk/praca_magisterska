using System.Collections.Generic;
using System.Linq;
using Assets.Editor.ExposeProperties;
using Assets.Editor.Konfiguracje;
using Assets.Skrypty;
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
      private DzialaniaNaMapie _dzialaniaNaMapie;
      private DzialaniaNaWezlach _dzialaniaNaWezlach;
      private Warstwa _poprzedniaWarstwa;
      public IList<Warstwa> _utworzoneWarstwy;

      private bool _pokazRogi = false;
      private bool _pokazRogiPoprzedniaWartosc = true;

      private float _zasiegZaburzenia = Konf.PoczStopienZaburzeniaWezlow;
      public int _rozmiarX = Konf.PoczRozmiarX;
      public int _rozmiarZ = Konf.PoczRozmiarZ;
      public float _rozpietosc = Konf.PoczRozpietosc;

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

      private Etap _etap = Etap.GenerowanieWezlow;
      private int _numerWybranejWarstwy = 0;

      [ExposeProperty]
      public string EtapTekst { get { return _etap.ToString(); } set {} } // musi byæ set ¿eby siê wyœwietla³o

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

      public PoziomEditor()
      {
         _dzialaniaNaMapie = new DzialaniaNaMapie(this);
         _dzialaniaNaWezlach = new DzialaniaNaWezlach(this);
         _utworzoneWarstwy = new List<Warstwa>();
      }
      
      public void OnEnable()
      {
         _poziom = target as Poziom;
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
            _pokazRogi = false;
            _pokazRogiPoprzedniaWartosc = true;
            PokazSciany = true;
            _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
            _etap = Etap.GenerowanieWezlow;
            _utworzoneWarstwy.Clear();
         }
         GUI.color = Color.white;

         if (_etap == Etap.GenerowanieWezlow)
         {
            EditorGUILayout.LabelField("Okreœl wymiary poziomu:", Konf.StylNaglowkaInspektora);
            _rozmiarX = EditorGUILayout.IntSlider("Rozmiar X", _rozmiarX, Konf.MinRozmiar, Konf.MaksRozmiar);
            _rozmiarZ = EditorGUILayout.IntSlider("Rozmiar Z", _rozmiarZ, Konf.MinRozmiar, Konf.MaksRozmiar);
            _rozpietosc = EditorGUILayout.Slider("Rozpiêtoœæ", _rozpietosc, Konf.MinRozpietosc, Konf.MaksRozpietosc);
            if (GUILayout.Button("Generuj wêz³y"))
            {
               _dzialaniaNaMapie.UsunWezlyRogiIKomorki();
               _dzialaniaNaWezlach.GenerujWezly();
               _etap = Etap.ZaburzanieWezlow;
            }
         }

         if ((_etap == Etap.ZaburzanieWezlow || _etap == Etap.TworzenieKomorekIRogow))
         {
            EditorGUILayout.LabelField("Okreœl stopieñ zaburzenia:", Konf.StylNaglowkaInspektora);
            _zasiegZaburzenia = EditorGUILayout.Slider("Stopieñ zaburzenia", _zasiegZaburzenia, 0f, 1f);
            if (GUILayout.Button("Zaburz wêz³y"))
            {
               _dzialaniaNaWezlach.ZaburzWezly(_zasiegZaburzenia * _rozpietosc, true);
               _etap = Etap.TworzenieKomorekIRogow;
            }
            if (GUILayout.Button("Utwórz komórki i rogi"))
            {
               _dzialaniaNaWezlach.UkryjWezly();
            _dzialaniaNaWezlach.GenerujKomorkiIRogi();
            _etap = Etap.TworzenieMapyWysokosci;
            }
         }

         if (_etap >= Etap.TworzenieMapyWysokosci)
         {
            PokazSciany = GUILayout.Toggle(PokazSciany, "Poka¿ œciany");
            _pokazRogi = GUILayout.Toggle(_pokazRogi, "Poka¿ rogi");
            if (_pokazRogi != _pokazRogiPoprzedniaWartosc)
            {
               _pokazRogiPoprzedniaWartosc = _pokazRogi;
               if (!_pokazRogi)
                  _dzialaniaNaMapie.UkryjRogi();
               else
                  _dzialaniaNaMapie.PokazRogi();
            }

         }

         if (_etap == Etap.TworzenieMapyWysokosci || _etap == Etap.RozdzielanieZiemiIWody)
         {
            GUILayout.BeginVertical(new GUIStyle
            {
               alignment = TextAnchor.MiddleCenter,
               margin = new RectOffset(100, 100, 0, 0)
            });
            if (_utworzoneWarstwy.Count > 0)
            {
               GUILayout.Label("Wybierz warstwê:");
               _numerWybranejWarstwy = GUILayout.SelectionGrid(_numerWybranejWarstwy,
                  _utworzoneWarstwy.ToList().Select(w => w.ToString()).ToArray(),
                  1);

               AktualnaWarstwa = _utworzoneWarstwy[_numerWybranejWarstwy];
               OdswiezZaznaczenieWarstwy();

               if (Poziom.AktualnaWarstwa != PoprzedniaWarstwa)
               {
                  PoprzedniaWarstwa = Poziom.AktualnaWarstwa;
                  if (Poziom.AktualnaWarstwa == Warstwa.Wysokosci)
                  {
                     _dzialaniaNaMapie.PokazWarstweWysokosci();
                     OdswiezZaznaczenieWarstwy();
                  }

                  if (Poziom.AktualnaWarstwa == Warstwa.ZiemiaWoda)
                  {
                     _dzialaniaNaMapie.PokazWarstweZiemiIWody();
                     OdswiezZaznaczenieWarstwy();
                  }
                  
               }
            }
            GUILayout.EndVertical();
            

            if (GUILayout.Button("Generuj wysokoœci"))
            {
               _dzialaniaNaMapie.GenerujWysokosci();
               if (!_utworzoneWarstwy.Contains(Warstwa.Wysokosci))
                  _utworzoneWarstwy.Add(Warstwa.Wysokosci);
               AktualnaWarstwa = Warstwa.Wysokosci;
               _dzialaniaNaMapie.PokazWarstweWysokosci();
               OdswiezZaznaczenieWarstwy();
               _etap = Etap.RozdzielanieZiemiIWody;
            } 
         }

         if (_etap == Etap.RozdzielanieZiemiIWody && GUILayout.Button("Rozdziel ziemiê i wodê"))
         {
            _dzialaniaNaMapie.RozdzielZiemieIWode();
            if (!_utworzoneWarstwy.Contains(Warstwa.ZiemiaWoda))
               _utworzoneWarstwy.Add(Warstwa.ZiemiaWoda);
            AktualnaWarstwa = Warstwa.ZiemiaWoda;
            _dzialaniaNaMapie.PokazWarstweZiemiIWody();
            OdswiezZaznaczenieWarstwy();
         }

      }

      public void OdswiezZaznaczenieWarstwy()
      {
         _numerWybranejWarstwy = _utworzoneWarstwy.IndexOf(AktualnaWarstwa);
      }
   }
}